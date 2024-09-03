namespace EDUHUNT_BE.DTOs
{
    public class AnswerDto
    {
        public int AnswerId { get; set; }
        public string AnswerText { get; set; } = default!;
        public int QuestionId { get; set; }

    }
}
