using EDUHUNT_BE.Data;
using EDUHUNT_BE.DTOs;
using EDUHUNT_BE.Helper;
using EDUHUNT_BE.Interfaces;
using EDUHUNT_BE.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EDUHUNT_BE.Repositories
{
    public class CodeVerifiesRepository : ICodeVerifies
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public CodeVerifiesRepository(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task CreateCodeVerify(CodeVerify codeVerify)
        {
            var user = await _userManager.FindByEmailAsync(codeVerify.Email);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            // Fetch any existing verification codes for the user
            var existingVerifications = _context.CodeVerifies.Where(v => v.UserId == user.Id);

            // If any exist, remove them
            if (existingVerifications.Any())
            {
                _context.CodeVerifies.RemoveRange(existingVerifications);
                await _context.SaveChangesAsync();
            }

            var Verify = new CodeVerify
            {
                Email = codeVerify.Email,
                UserId = user.Id,
                Code = codeVerify.Code,
                Id = Guid.NewGuid(),
                ExpirationTime = DateTime.UtcNow.AddSeconds(30),
            };

            _context.CodeVerifies.Add(Verify);
            await _context.SaveChangesAsync();
        }

        public async Task<ServiceResponses.GeneralResponse> ResetPassword([FromBody] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new ServiceResponses.GeneralResponse(false, "User not found");
            }

            // Generate a 6-digit random number
            var random = new Random();
            var code = random.Next(100000, 999999).ToString();

            var verify = new CodeVerify
            {
                Email = email,
                Code = code,
            };

            await CreateCodeVerify(verify);

            var message = $@"
            <html>
            <head>
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        display: flex;
                        justify-content: center;
                        align-items: center;
                        height: 100vh;
                        margin: 0;
                    }}

                    h2 {{
                        color: #333;
                    }}

                    p {{
                        color: #666;
                    }}

                    .button {{
                        display: inline-block;
                        padding: 10px 20px;
                        color: white;
                        background-color: #007BFF;
                        border: none;
                        border-radius: 5px;
                        text-decoration: none;
                    }}

                    .container {{
                        text-align: center;
                        border: 50px solid #007BFF;
                        padding: 40px;
                    }}
                </style>
            </head>
            <body>
                <div class=""container"">
                    <h2>Reset your password</h2>
                    <p>Welcome to <strong>EduHunt</strong>. Enter this code in your open browser window to reset your password</p>
                    <div>
                        {code}
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

            // Please replace SendMail.SendEmail with your own method to send emails
            SendMail.SendEmail(email, "Reset your password", message, "");

            return new ServiceResponses.GeneralResponse(true, "Send mail successfully");
        }


        public async Task<ServiceResponses.GeneralResponse> VerifyCode(CodeVerify codeVerify)
        {
            codeVerify.Id = new Guid();
            var Verify = await _context.CodeVerifies.FirstOrDefaultAsync(c => c.Email == codeVerify.Email && c.Code == codeVerify.Code);

            if (Verify == null)
            {
                return new ServiceResponses.GeneralResponse(false,"Not Found");
            }

            if (DateTime.UtcNow > Verify.ExpirationTime)
            {
                _context.CodeVerifies.Remove(Verify);
                await _context.SaveChangesAsync();
                return new ServiceResponses.GeneralResponse(false, "Not Found");
            }

            return new ServiceResponses.GeneralResponse(true, "Verify Successfully");
        }
    }
}
