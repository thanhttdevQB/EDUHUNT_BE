using EDUHUNT_BE.Data;
using EDUHUNT_BE.DTOs;
using EDUHUNT_BE.Interfaces;
using EDUHUNT_BE.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static EDUHUNT_BE.DTOs.ServiceResponses;

namespace EDUHUNT_BE.Repositories
{
    public class AccountRepository : IUserAccount
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration config;
        private readonly AppDbContext context;

        public AccountRepository(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration config,
            AppDbContext context) // Inject the AppDbContext through the constructor
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.config = config;
            this.context = context;
            // Assign the injected AppDbContext to the private field
        }

        public async Task<GeneralResponse> CreateAccount(UserDto userDto)
        {
            if (userDto is null) return new GeneralResponse(false, "Model is empty");
            var newUser = new ApplicationUser()
            {
                Name = userDto.Name,
                Email = userDto.Email,
                PasswordHash = userDto.Password,
                UserName = userDto.Email,
                EmailConfirmed = userDto.EmailConfirmed,
            };
            var user = await userManager.FindByEmailAsync(newUser.Email);
            if (user is not null) return new GeneralResponse(false, "User registered already");

            var createUser = await userManager.CreateAsync(newUser, userDto.Password);

            if (!createUser.Succeeded)
            {
                // Lấy danh sách lỗi từ quá trình tạo người dùng
                var error = createUser.Errors.Select(e => e.Description).FirstOrDefault();

                // Trả về thông báo lỗi
                return new GeneralResponse(false, error);
            }

            //Assign Default Role : 1: User, 2: Scholarship Provider, 3: Mentor 
            switch (userDto.RoleId)
            {
                case 1:
                    await AssignRole(newUser, "User");
                    break;
                case 2:
                    await AssignRole(newUser, "Scholarship Provider");
                    break;
                case 3:
                    await AssignRole(newUser, "Mentor");
                    break;
                default:
                    await AssignRole(newUser, "Admin");
                    break;
            }

            try
            {
                var profile = new Profile
                {
                    UserId = Guid.Parse(newUser.Id),
                    UrlAvatar = "https://dentistry.co.uk/app/uploads/2020/11/anonymous-avatar-icon-25.png"
                    // Assign other properties as needed
                };

                context.Profile.Add(profile);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Handle the exception here
                // You can log the exception or perform any other necessary actions
                // For example, you can throw a custom exception or return an error response
                throw new Exception("An error occurred while saving the profile.", ex);
            }
            return new GeneralResponse(true, "Account Created");
        }

        private async Task AssignRole(ApplicationUser user, string roleName)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role is null)
            {
                await roleManager.CreateAsync(new IdentityRole() { Name = roleName });
                await userManager.AddToRoleAsync(user, roleName);
            }
            else
            {
                await userManager.AddToRoleAsync(user, roleName);
            }
        }

        public async Task<GeneralResponse> ChangePassword(string userId, string currentPassword, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(currentPassword) || string.IsNullOrWhiteSpace(newPassword))
                return new GeneralResponse(false, "Invalid parameters");

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return new GeneralResponse(false, "User not found");

            var result = await userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!result.Succeeded)
            {
                var error = result.Errors.Select(e => e.Description).FirstOrDefault();
                return new GeneralResponse(false, error);
            }
            return new GeneralResponse(true, "Password changed successfully");
        }

        public async Task<LoginResponse> LoginAccount(LoginDto loginDto)
        {
            if (loginDto == null)
                return new LoginResponse(false, null!, null!, "Login container is empty");

            var getUser = await userManager.FindByEmailAsync(loginDto.Email);
            if (getUser is null)
                return new LoginResponse(false, null!, null!, "User not found");

            if (!getUser.EmailConfirmed)
                return new LoginResponse(false, null!, null!, "Email not confirmed. Please confirm your email before logging in.");

            bool checkUserPasswords = await userManager.CheckPasswordAsync(getUser, loginDto.Password);
            if (!checkUserPasswords)
                return new LoginResponse(false, null!, null!, "Invalid email/password");

            var getUserRole = await userManager.GetRolesAsync(getUser);
            var userSession = new UserSession(getUser.Id, getUser.Name, getUser.Email, getUserRole.First());
            string token = GenerateToken(userSession);

            var iduuid = Guid.Parse(getUser.Id);
            var profile = await context.Profile.FirstOrDefaultAsync(Profile => Profile.UserId == iduuid);

            return new LoginResponse(true, token!, getUser.Id, "Login completed");
        }

        public Task<LoginResponse> LogoutAccount()
        {
            throw new NotImplementedException();
        }

        private string GenerateToken(UserSession user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };
            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: userClaims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<GeneralResponse> ForgotPassword(string email, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(newPassword))
                return new GeneralResponse(false, "Invalid parameters");

            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
                return new GeneralResponse(false, "User not found");

            var passwordResetToken = await userManager.GeneratePasswordResetTokenAsync(user);

            var result = await userManager.ResetPasswordAsync(user, passwordResetToken, newPassword);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                var errorMessage = string.Join(", ", errors);
                return new GeneralResponse(false, errorMessage);
            }

            return new GeneralResponse(true, "Password reset successfully");
        }

        public async Task<List<ListUserDto>> ListUser()
        {
            var users = await userManager.Users.ToListAsync();
            var userDtos = new List<ListUserDto>();

            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                var iduuid = Guid.Parse(user.Id);
                var profile = await context.Profile.FirstOrDefaultAsync(Profile => Profile.UserId == iduuid);
                var userDto = new ListUserDto
                {
                    Id = user.Id,
                    Name = profile.UserName,
                    Email = user.Email,
                    Role = roles.ToList()
                };

                userDtos.Add(userDto);
            }

            return userDtos;
        }

        public async Task<DeleteUserResponse> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user is null) return new DeleteUserResponse(false, "User not found");

            // Convert user.Id to Guid since the profile uses Guid for UserId
            var userIdGuid = Guid.Parse(user.Id);

            // Find the profile associated with the user
            var profile = await context.Profile.FirstOrDefaultAsync(p => p.UserId == userIdGuid);

            // If a profile exists, remove it
            if (profile != null)
            {
                context.Profile.Remove(profile);
                // Save changes to the database to reflect the removal of the profile
                await context.SaveChangesAsync();
            }

            // Proceed to delete the user
            var deleteUser = await userManager.DeleteAsync(user);
            if (!deleteUser.Succeeded)
            {
                var errors = deleteUser.Errors.Select(e => e.Description);
                var errorMessage = string.Join(", ", errors);
                return new DeleteUserResponse(false, errorMessage);
            }
            return new DeleteUserResponse(true, "User and associated profile deleted");
        }

        public async Task<AccessDto> GetAccessByUserId(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            var access = new AccessDto
            {
                UserId = userId,
                Email = user.Email,
                Name = user.Name,
                FirstTimeAccessed = user.FirstTimeAccessed,
            };
            return access;
        }

        public async Task<GeneralResponse> UpdateAccess(AccessDto accessDto)
        {
            var user = await userManager.FindByIdAsync(accessDto.UserId);
            if (user == null)
            {
                return new GeneralResponse(false, "User not found");
            }
            user.FirstTimeAccessed = accessDto.FirstTimeAccessed;
            var updateUser = await userManager.UpdateAsync(user);
            return updateUser.Succeeded ? new GeneralResponse(true, "Access updated successfully") : new GeneralResponse(false, "Failed to update access");
        }
    }
}
