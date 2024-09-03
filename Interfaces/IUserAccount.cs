using EDUHUNT_BE.DTOs;
using static EDUHUNT_BE.DTOs.ServiceResponses;

namespace EDUHUNT_BE.Interfaces
{
    public interface IUserAccount
    {
        Task<GeneralResponse> CreateAccount(UserDto userDto);
        Task<GeneralResponse> ChangePassword(string userId, string currentPassword, string newPassword);
        Task<GeneralResponse> ForgotPassword(string email, string newPassword);
        Task<LoginResponse> LoginAccount(LoginDto loginDto);
        Task<LoginResponse> LogoutAccount();
        Task<List<ListUserDto>> ListUser();
        Task<AccessDto> GetAccessByUserId(string userId);
        Task<GeneralResponse> UpdateAccess(AccessDto accessDto);
        Task<DeleteUserResponse> DeleteUser(string id);
    }
}
