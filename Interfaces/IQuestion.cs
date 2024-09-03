using EDUHUNT_BE.DTOs;
using static EDUHUNT_BE.DTOs.ServiceResponses;

namespace EDUHUNT_BE.Interfaces
{
    public interface IQuestion
    {
        Task<ICollection<QuestionDto>> GetQuestions();
        Task<QuestionDto> GetQuestionById(int questionId);
        Task<GeneralResponse> CreateQuestion(QuestionDto question);
        Task<GeneralResponse> UpdateQuestion(QuestionDto question);
        Task<GeneralResponse> DeleteQuestion(int questionId);
        Task<ICollection<QuestionAnswerDto>> GetQuestionWithAnswer();
    }
}
