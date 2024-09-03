namespace EDUHUNT_BE.Models;

public partial class Answer
{
    public int AnswerId { get; set; }

    public int QuestionId { get; set; }

    public string AnswerText { get; set; } = default!;

    public virtual Question Question { get; set; } = default!;

    public virtual ICollection<SurveyAnswer> SurveyAnswers { get; set; } = default!;

    internal object Select(Func<object, Answer> value)
    {
        throw new NotImplementedException();
    }
}
