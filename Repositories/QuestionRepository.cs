using AutoMapper;
using EDUHUNT_BE.Data;
using EDUHUNT_BE.DTOs;
using EDUHUNT_BE.Interfaces;
using EDUHUNT_BE.Models;
using Microsoft.EntityFrameworkCore;
using static EDUHUNT_BE.DTOs.ServiceResponses;


namespace EDUHUNT_BE.Repositories
{
    public class QuestionRepository : IQuestion
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public QuestionRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<GeneralResponse> CreateQuestion(QuestionDto question)
        {
            var questionDto = new Question
            {
                QuestionId = question.QuestionId,
                Content = question.Content,
            };
            await _context.Questions.AddAsync(questionDto);
            await _context.SaveChangesAsync();
            return new GeneralResponse(true, "Question created successfully.");
        }

        public async Task<GeneralResponse> DeleteQuestion(int questionId)
        {
            var question = _context.Questions.Where(q => q.QuestionId == questionId).FirstOrDefault();
            if (question == null)
            {
                return new GeneralResponse(false, "Question not found");
            }
            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
            return new GeneralResponse(true, "Question deleted successfully.");
        }

        public async Task<QuestionDto> GetQuestionById(int questionId)
        {
            var question = await _context.Questions.Where(q => q.QuestionId == questionId).FirstOrDefaultAsync();
            if (question == null)
            {
                return null!;
            }
            var questionDto = new QuestionDto
            {
                QuestionId = questionId,
                Content = question.Content,
            };
            return questionDto;
        }

        public async Task<ICollection<QuestionDto>> GetQuestions()
        {
            var questions =await _context.Questions.OrderBy(q => q.QuestionId).ToListAsync();
            List<QuestionDto> result = new List<QuestionDto>();
            foreach (var question in questions)
            {
                var listQues = new QuestionDto
                {
                    QuestionId = question.QuestionId,
                    Content = question.Content,
                };
                result.Add(listQues);
            }
            return result;
        }

        public async Task<ICollection<QuestionAnswerDto>> GetQuestionWithAnswer()
        {
            var questions = await _context.Questions.Include(q => q.Answers).OrderBy(q => q).ToListAsync();
            var questionWithAnswers = _mapper.Map<List<QuestionAnswerDto>>(questions);
            return questionWithAnswers;
        }

        public async Task<GeneralResponse> UpdateQuestion(QuestionDto question)
        {
            var questionToUpdate = _context.Questions.Where(q => q.QuestionId == question.QuestionId).FirstOrDefault();
            if (questionToUpdate == null)
            {
                return new GeneralResponse(false, "Question not found");
            }

            questionToUpdate.Content = question.Content;
            _context.Questions.Update(questionToUpdate);
            await _context.SaveChangesAsync();
            return new GeneralResponse(true, "Question updated successfully.");
        }
    }
}