using EDUHUNT_BE.Data;

namespace EDUHUNT_BE.Models
{
    public partial class StudentType
    {
        public int StudentTypeId { get; set; }

        public string TypeName { get; set; } = default!;

        public string Money { get; set; } = default!;

        public string Study { get; set; } = default!;

        public virtual ICollection<ApplicationUser> Users { get; set; } = default!;
    }
}