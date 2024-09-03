namespace EDUHUNT_BE.Models
{
    public class RentMS
    {
        public Guid Id { get; set; }

        public Guid MentorId { get; set; }

        public Guid StudentId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
