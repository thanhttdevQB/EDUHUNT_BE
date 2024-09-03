namespace EDUHUNT_BE.DTOs
{
    public class SurveyDto
    {
        public string UserId { get; set; }
        public List<GetSurvey> SurveyAnswers { get; set; } = new List<GetSurvey>();
    }
    public class GetSurvey
    {
        public string Question { get; set; }
        public string Answer { get; set; }
    }
}
