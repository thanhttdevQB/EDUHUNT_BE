namespace EDUHUNT_BE.Models;

public partial class Question
{
    public int QuestionId { get; set; }

    public string Content { get; set; } = default!;

    public virtual ICollection<Answer> Answers { get; set; } = default!;
}
