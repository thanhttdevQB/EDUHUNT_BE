using Microsoft.AspNetCore.Mvc;
using EDUHUNT_BE.Models;
using EDUHUNT_BE.Interfaces;

namespace EDUHUNT_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CodeVerifiesController : ControllerBase
    {
        private readonly ICodeVerifies _codeVerifies;

        public CodeVerifiesController(ICodeVerifies codeVerifies)
        {
            _codeVerifies = codeVerifies;
        }

        [HttpPost("Save")]
        public async Task CreateCodeVerify(CodeVerify codeVerify)
        {
            await _codeVerifies.CreateCodeVerify(codeVerify);
        }

        // POST: api/CodeVerifies/resetpassword
        [HttpPost("resetpassword")]
        public async Task<ActionResult<bool>> ResetPassword([FromBody] string email)
        {
            return Ok(await _codeVerifies.ResetPassword(email));
        }


        // GET: api/CodeVerifies/Verify
        [HttpPost("Verify")]
        public async Task<ActionResult<bool>> VerifyCode(CodeVerify codeVerify)
        {
            return Ok(await _codeVerifies.VerifyCode(codeVerify));
        }
    }
}
