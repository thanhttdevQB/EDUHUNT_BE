namespace EDUHUNT_BE.DTOs
{
    public class ServiceResponses
    {
        public record class GeneralResponse(bool Flag, string Message);
        public record class LoginResponse(bool Flag, string Token, string UserId, string Message);
        public record class DeleteUserResponse(bool Flag, string Message);
    }
}
