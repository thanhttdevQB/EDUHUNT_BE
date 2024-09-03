using System.ComponentModel.DataAnnotations;

namespace EDUHUNT_BE.Models
{
    public class ScholarshipInfo
    {
        public Guid Id { get; set; }

        [Required]
        public string Budget { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public string SchoolName { get; set; }

        [Required]
        public string Level { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }

        #nullable enable
        public string? AuthorId { get; set; }
        public bool? IsInSite { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsApproved { get; set; } = false;
        public virtual ICollection<ScholarshipCategory> ScholarshipCategories { get; set; } = default!;

    }
}