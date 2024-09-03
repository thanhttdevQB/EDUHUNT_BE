namespace EDUHUNT_BE.DTOs
{
    public class ScholarshipDto
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Budget { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public string School_name { get; set; } = string.Empty;

        public string Level { get; set; } = string.Empty;

        public string Url { get; set; } = string.Empty;
    }
}
