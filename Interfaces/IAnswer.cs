using EDUHUNT_BE.DTOs;
using static EDUHUNT_BE.DTOs.ServiceResponses;

namespace EDUHUNT_BE.Interfaces
{
    public interface IAnswer
    {
        Task<ICollection<AnswerDto>> GetAnswers();
        Task<AnswerDto> GetAnswerById(int answerId);
        Task<GeneralResponse> CreateAnswer(AnswerDto answer);
        Task<GeneralResponse> UpdateAnswer(AnswerDto answer);
        Task<GeneralResponse> DeleteAnswer(int answerId);
    }
}