using EDUHUNT_BE.Data;
using EDUHUNT_BE.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using EDUHUNT_BE.Interfaces;
using EDUHUNT_BE.DTOs;

namespace EDUHUNT_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserAccount userAccount;
        private readonly UserManager<ApplicationUser> userManager;

        public AccountController(IUserAccount userAccount, UserManager<ApplicationUser> userManager)
        {
            this.userAccount = userAccount;
            this.userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDto userDto)
        {
            var response = await userAccount.CreateAccount(userDto);

            if (response.Flag)
            {
                var user = await userManager.FindByEmailAsync(userDto.Email);
                if (user != null)
                {
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account", new { token, email = user.Email }, Request.Scheme);

                    var message = $@"
                    <html>
                    <head>
                        <style>
                            body {{font-family: Arial, sans-serif;
                                display: flex;
                                justify-content: center;
                                align-items: center;
                                height: 100vh;
                                margin: 0;
                            }}

                            h2 {{color: #333;
                            }}

                            p {{color: #666;
                            }}

                            .button {{display: inline-block;
                                padding: 10px 20px;
                                color: white;
                                background-color: #007BFF;
                                border: none;
                                border-radius: 5px;
                                text-decoration: none;
                            }}

                            .container {{text-align: center;
                                border: 50px solid #007BFF;
                                padding:40px;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class=""container"">
                        <h2>Confirm your email address</h2>
                        <p>Welcome to <strong>EduHunt</strong>. Enter the button below, and we will help
                            you get signed in.</p>
                        <div>
                            <a href='{HtmlEncoder.Default.Encode(confirmationLink)}' class='button'>Verify Email</a>
                        </div>
                        <p>If you did not request this email, there is nothing to worry about; you can safely ignore it.</p>

                        <img src='https://img.freepik.com/free-vector/bird-colorful-gradient-design-vector_343694-2506.jpg' width='120'
                            height='70' alt='Sky' />

                        <p>
                            ©2024 EduHunt Project, Da Nang, Viet Nam <br />
                            <br />
                            All rights reserved.
                        </p>
                    </div>
                    </body>
                    </html>";
                    SendMail.SendEmail(userDto.Email, "Confirm your email", message, "");
                }
            }

            return Ok(response);
        }

        [HttpGet("AccessPage")]
        public async Task<AccessDto> GetAccessByUserId(string userId)
        {
            var accessDto = await userAccount.GetAccessByUserId(userId);
            return accessDto;
        }

        [HttpPut("AccessPage")]
        public async Task<IActionResult> UpdateAccessPage(AccessDto accessDto)
        {
            var response = await userAccount.UpdateAccess(accessDto);
            return Ok(response);
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
                return BadRequest();

            var result = await userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
                return Redirect("http://localhost:3000/login");
            else
                return BadRequest();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var response = await userAccount.LoginAccount(loginDto);
            return Ok(response);
        }

        [HttpPost("changePassword")]
        public async Task<IActionResult> ChangePassword(PasswordDto passwordDto)
        {
            if (passwordDto == null)
                return BadRequest("Password data is null");

            var response = await userAccount.ChangePassword(passwordDto.Id, passwordDto.Password, passwordDto.NewPassword);
            return Ok(response);
        }

        [HttpPost("forgotPassword")]
        public async Task<IActionResult> ForgotPassword(PasswordDto passwordDto)
        {
            if (passwordDto == null)
                return BadRequest("Password data is null");

            var response = await userAccount.ForgotPassword(passwordDto.Email, passwordDto.NewPassword);
            return Ok(response);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var response = await userAccount.LogoutAccount();
            return Ok(response);
        }

        [HttpGet("listuser")]
        public async Task<IActionResult> ListUser()
        {
            var response = await userAccount.ListUser();
            return Ok(response);
        }


        [HttpDelete("deleteuser/{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] string id)
        {
            var response = await userAccount.DeleteUser(id);
            return Ok(response);
        }

    }
}
