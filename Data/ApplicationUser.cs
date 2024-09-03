using EDUHUNT_BE.Models;
using Microsoft.AspNetCore.Identity;

namespace EDUHUNT_BE.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public bool FirstTimeAccessed { get; set; } = false;
    }
}
