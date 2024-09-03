namespace EDUHUNT_BE.DTOs
{
    public class SurveyAnswerDto
    {
        public int SurveyId { get; set; }   
        public string Title {  get; set; }
        public string Description {  get; set; }
        public DateTime CreateAt { get; set; }
        public string UserId { get; set; }
        public List<int> AnswerIds { get; set; } = default!;
    }
}
