using AutoMapper;
using EDUHUNT_BE.Data;
using EDUHUNT_BE.DTOs;
using EDUHUNT_BE.Interfaces;
using EDUHUNT_BE.Models;
using Microsoft.EntityFrameworkCore;
using static EDUHUNT_BE.DTOs.ServiceResponses;

namespace EDUHUNT_BE.Repositories
{
    public class SurveyRepository : ISurvey
    {
        private readonly AppDbContext _AppDbContext;
        private readonly IMapper _mapper;

        public SurveyRepository(AppDbContext AppDbContext, IMapper mapper)
        {
            _AppDbContext = AppDbContext;
            _mapper = mapper;
        }

        public async Task<GeneralResponse> Delete(int surveyId)
        {
            if (_AppDbContext == null)
            {
                return new GeneralResponse(false, "Survey context is null.");
            }

            try
            {
                var surveys = await _AppDbContext.Surveys.FirstOrDefaultAsync(sa => sa.SurveyId == surveyId);
                if (surveys == null)
                {
                    return new GeneralResponse(false, "Survey does not exist.");
                }

                var existingSurveyAnswers = await _AppDbContext.SurveyAnswers.Where(sa => sa.SurveyId == surveyId).ToListAsync();
                _AppDbContext.SurveyAnswers.RemoveRange(existingSurveyAnswers);
                _AppDbContext.Surveys.Remove(surveys);
                await _AppDbContext.SaveChangesAsync();

                return new GeneralResponse(true, "Survey deleted successfully.");
            }
            catch (Exception ex)
            {
                return new GeneralResponse(false, ex.Message);
            }
        }

        public async Task<SurveyDto> Get(int id)
        {
            if (_AppDbContext == null)
            {
                return null!;
            }

            var survey = await _AppDbContext.Surveys.FirstOrDefaultAsync(s => s.SurveyId == id);
            if (survey == null)
            {
                return null!;
            }

            return _mapper.Map<SurveyDto>(survey);
        }

        public async Task<GeneralResponse> CreateSurvey(SurveyAnswerDto datasurvey)
        {
            if (_AppDbContext == null)
            {
                return new GeneralResponse(false, "Survey context is null.");
            }

            try
            {
                var check_user = await _AppDbContext.Users.FindAsync(datasurvey.UserId);
                if (check_user == null)
                {
                    return new GeneralResponse(false, "User not found.");
                }

                var surveycheck = await _AppDbContext.Surveys.FirstOrDefaultAsync(sa => sa.UserId == check_user.Id);
                if (surveycheck != null)
                {
                    return new GeneralResponse(false, "User already has a survey.");
                }

                datasurvey.CreateAt = DateTime.Now;
                var new_survey = _mapper.Map<Survey>(datasurvey);

                _AppDbContext.Surveys.Add(new_survey);
                await _AppDbContext.SaveChangesAsync();

                foreach (var id in datasurvey.AnswerIds)
                {
                    var is_answer = await _AppDbContext.Answers.FindAsync(id) != null;
                    if (is_answer)
                    {
                        var survey_answer = new SurveyAnswer
                        {
                            SurveyId = new_survey.SurveyId,
                            AnswerId = id
                        };
                        _AppDbContext.SurveyAnswers.Add(survey_answer);
                    }
                }
                await _AppDbContext.SaveChangesAsync();

                return new GeneralResponse(true, "Survey created successfully.");
            }
            catch (Exception ex)
            {
                return new GeneralResponse(false, ex.Message);
            }
        }

        public async Task<GeneralResponse> UpdateSurvey(SurveyAnswerDto surveyobj)
        {
            if (_AppDbContext == null)
            {
                return new GeneralResponse(false, "Survey context is null.");
            }

            try
            {
                var survey = await _AppDbContext.Surveys.FirstOrDefaultAsync(sa => sa.SurveyId == surveyobj.SurveyId);
                if (survey == null)
                {
                    return new GeneralResponse(false, "Survey does not exist.");
                }

                survey.Title = surveyobj.Title;
                survey.Description = surveyobj.Description;
                survey.CreateAt = DateTime.Now;
                _AppDbContext.Surveys.Update(survey);

                var existingSurveyAnswers = await _AppDbContext.SurveyAnswers.Where(sa => sa.SurveyId == surveyobj.SurveyId).ToListAsync();
                _AppDbContext.SurveyAnswers.RemoveRange(existingSurveyAnswers);

                foreach (int answerid in surveyobj.AnswerIds)
                {
                    var answer = await _AppDbContext.Answers.FirstOrDefaultAsync(a => a.AnswerId == answerid);
                    if (answer != null)
                    {
                        var new_answer = new SurveyAnswer
                        {
                            AnswerId = answerid,
                            SurveyId = surveyobj.SurveyId,
                        };
                        _AppDbContext.SurveyAnswers.Add(new_answer);
                    }
                }
                await _AppDbContext.SaveChangesAsync();

                return new GeneralResponse(true, "Survey updated successfully.");
            }
            catch (Exception ex)
            {
                return new GeneralResponse(false, ex.Message);
            }
        }

        public async Task<ICollection<SurveyDto>> GetAll()
        {
            if (_AppDbContext == null)
            {
                return null!;
            }

            var surveys = await _AppDbContext.Surveys
                .Include(s => s.SurveyAnswers)
                .Select(s => new SurveyDto
                {
                    UserId = s.UserId,
                    SurveyAnswers = s.SurveyAnswers.Select(sa => new GetSurvey
                    {
                        Question = sa.Answer.Question.Content,
                        Answer = sa.Answer.AnswerText
                    }).ToList()
                }).ToListAsync();

            return surveys;
        }

        public async Task<SurveyDto> GetByUserId(string userId)
        {
            var survey = await _AppDbContext.Surveys
                .Include(s => s.SurveyAnswers)
                    .ThenInclude(sa => sa.Answer)
                        .ThenInclude(a => a.Question)
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (survey == null)
            {
                return null;
            }

            var surveyDto = new SurveyDto
            {
                UserId = survey.UserId,
                SurveyAnswers = survey.SurveyAnswers.Select(sa => new GetSurvey
                {
                    Question = sa.Answer.Question.Content,
                    Answer = sa.Answer.AnswerText
                }).ToList()
            };

            return surveyDto;
        }
    }
}