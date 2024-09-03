using EDUHUNT_BE.Data;

namespace EDUHUNT_BE.Models
{
    public partial class Survey
    {
        public int SurveyId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string UserId { get; set; }

        public DateTime CreateAt { get; set; }

        public virtual ICollection<SurveyAnswer> SurveyAnswers { get; set; } = default!;
    }
}