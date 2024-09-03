using EDUHUNT_BE.Data;
using EDUHUNT_BE.DTOs;
using EDUHUNT_BE.Interfaces;
using EDUHUNT_BE.Models;
using Microsoft.EntityFrameworkCore;
using static EDUHUNT_BE.DTOs.ServiceResponses;


namespace EDUHUNT_BE.Repositories
{
    public class AnswerRepository : IAnswer
    {
        private readonly AppDbContext _context;
        public AnswerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GeneralResponse> CreateAnswer(AnswerDto answer)
        {
            // Check if the QuestionId exists
            var questionExists = await _context.Questions.AnyAsync(q => q.QuestionId == answer.QuestionId);
            if (!questionExists)
            {
                return new GeneralResponse(false, "Question not found");
            }

            var answerEntity = new Answer
            {
                AnswerId = answer.AnswerId,
                AnswerText = answer.AnswerText,
                QuestionId = answer.QuestionId
            };
            await _context.Answers.AddAsync(answerEntity);
            await _context.SaveChangesAsync();
            return new GeneralResponse(true, "Answer created successfully.");
        }

        public async Task<GeneralResponse> DeleteAnswer(int answerId)
        {
            var answer = await _context.Answers.Where(a => a.AnswerId == answerId).FirstOrDefaultAsync();
            if (answer == null)
            {
                return new GeneralResponse(false, "Answer not found");
            }
            _context.Answers.Remove(answer);
            await _context.SaveChangesAsync();
            return new GeneralResponse(true, "Answer deleted successfully.");
        }

        public async Task<AnswerDto> GetAnswerById(int answerId)
        {
            var answer = await _context.Answers.Where(a => a.AnswerId == answerId).FirstOrDefaultAsync();
            if (answer == null)
            {
                return null!;
            }
            var answerDto = new AnswerDto
            {
                AnswerId = answer.AnswerId,
                AnswerText = answer.AnswerText,
                QuestionId = answer.QuestionId
            };
            return answerDto;
        }

        public async Task<ICollection<AnswerDto>> GetAnswers()
        {
            var answers = await _context.Answers.OrderBy(a => a.AnswerId).ToListAsync();
            List<AnswerDto> result = new List<AnswerDto>();
            foreach (var answer in answers)
            {
                var answerDto = new AnswerDto
                {
                    AnswerId = answer.AnswerId,
                    AnswerText = answer.AnswerText,
                    QuestionId = answer.QuestionId
                };
                result.Add(answerDto);
            }
            return result;
        }

        public async Task<GeneralResponse> UpdateAnswer(AnswerDto answer)
        {
            // Check if the QuestionId exists
            var questionExists = await _context.Questions.AnyAsync(q => q.QuestionId == answer.QuestionId);
            if (!questionExists)
            {
                return new GeneralResponse(false, "Question not found");
            }

            var answerToUpdate = await _context.Answers.Where(a => a.AnswerId == answer.AnswerId).FirstOrDefaultAsync();
            if (answerToUpdate == null)
            {
                return new GeneralResponse(false, "Answer not found");
            }
            answerToUpdate.AnswerText = answer.AnswerText;
            answerToUpdate.QuestionId = answer.QuestionId;
            _context.Answers.Update(answerToUpdate);
            await _context.SaveChangesAsync();
            return new GeneralResponse(true, "Answer updated successfully.");
        }
    }
}