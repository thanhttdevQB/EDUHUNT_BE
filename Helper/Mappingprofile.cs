using EDUHUNT_BE.DTOs;
using EDUHUNT_BE.Models;

namespace EDUHUNT_BE.Helper
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            CreateMap<Question, QuestionDto>().ReverseMap();
            CreateMap<Answer, AnswerDto>().ReverseMap();
            CreateMap<StudentTypeDto, StudentType>().ReverseMap();
            CreateMap<SurveyDto, Survey>().ReverseMap();
            CreateMap<SurveyAnswerDto, Survey>().ReverseMap();
            CreateMap<QuestionAnswerDto, Question>().ReverseMap();
        }
    }
}