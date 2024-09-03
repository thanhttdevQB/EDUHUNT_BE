namespace EDUHUNT_BE.DTOs
{
    public class StudentTypeDto
    {
        public int StudentTypeId { get; set; }

        public string TypeName { get; set; } = default!;

        public string Money { get; set; } = default!;

        public string Study { get; set; } = default!;
    }
}
