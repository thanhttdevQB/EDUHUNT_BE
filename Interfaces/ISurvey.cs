using EDUHUNT_BE.DTOs;
using static EDUHUNT_BE.DTOs.ServiceResponses;

namespace EDUHUNT_BE.Interfaces
{
    public interface ISurvey
    {
        Task<GeneralResponse> CreateSurvey(SurveyAnswerDto survey);
        Task<SurveyDto> Get(int id);
        Task<GeneralResponse> UpdateSurvey(SurveyAnswerDto surveyObj);
        Task<GeneralResponse> Delete(int surveyId);
        Task<ICollection<SurveyDto>> GetAll();
        Task<SurveyDto> GetByUserId(string userId);
    }
}