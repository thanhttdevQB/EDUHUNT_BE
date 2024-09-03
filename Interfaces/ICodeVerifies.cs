using EDUHUNT_BE.Models;
using Microsoft.AspNetCore.Mvc;
using static EDUHUNT_BE.DTOs.ServiceResponses;

namespace EDUHUNT_BE.Interfaces
{
    public interface ICodeVerifies
    {
        public Task CreateCodeVerify(CodeVerify codeVerify);
        public Task<GeneralResponse> ResetPassword([FromBody] string email);
        public Task<GeneralResponse> VerifyCode(CodeVerify codeVerify);
    }
}
