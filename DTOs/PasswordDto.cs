using System.ComponentModel.DataAnnotations;

namespace EDUHUNT_BE.DTOs
{
    public class PasswordDto
    {
        public string Id { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;
    }
}
