namespace EDUHUNT_BE.DTOs
{
    public class QuestionAnswerDto
    {
        public int QuestionId { get; set; }
        public string Content { get; set; }
        public List<AnswerDto> Answers { get; set; }
    }
}
