namespace EDUHUNT_BE.Models;

public partial class SurveyAnswer
{
    public int SurveyAnswerId { get; set; }

    public int SurveyId { get; set; }

    public int AnswerId { get; set; }

    public virtual Answer Answer { get; set; } = default!;

    public virtual Survey Survey { get; set; } = default!;
}
