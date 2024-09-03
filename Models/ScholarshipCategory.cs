namespace EDUHUNT_BE.Models
{
    public class ScholarshipCategory
    {
        public int Id { get; set; }

        public Guid ScholarshipId { get; set; }

        public int CategoryId { get; set; }

        public virtual ScholarshipInfo ScholarshipInfo { get; set; }

        public virtual Category Category { get; set; }
    }
}
