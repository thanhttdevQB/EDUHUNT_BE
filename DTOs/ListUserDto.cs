namespace EDUHUNT_BE.DTOs
{
    public class ListUserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<string> Role { get; set; } = new List<string>();
    }
}
