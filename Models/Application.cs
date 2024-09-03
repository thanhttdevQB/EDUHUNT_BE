namespace EDUHUNT_BE.Models
{
    public class Application
    {
        public Guid Id { get; set; }
        public Guid StudentID { get; set; }
        public Guid ScholarshipID { get; set; }    
        public string StudentCV { get; set; }
        public string Status { get; set; }
        public string MeetingURL { get; set; }
        public DateTime? StudentChooseDay { get; set; }
        public DateTime? ScholarshipProviderAvailableStartDate { get; set; }
        public DateTime? ScholarshipProviderAvailableEndDate { get; set; }
        public string ApplicationReason { get; set; }
        public string AttachFile { get; set; }
    }
}
