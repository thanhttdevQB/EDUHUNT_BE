using EDUHUNT_BE.DTOs;
using EDUHUNT_BE.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace survey.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestion _questionRepository;
        public QuestionController(IQuestion questionRepository)
        {
            _questionRepository = questionRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetQuestion() {
            var questions = await _questionRepository.GetQuestions();
            if (questions == null || !questions.Any())
            {
                return NotFound("No questions found.");
            }
            return Ok(questions);
        }

        [HttpGet("with-answers")]
        public async Task<IActionResult> GetQuestionWithAnswers()
        {
            var questionsWithAnswers = await _questionRepository.GetQuestionWithAnswer();
            if (questionsWithAnswers == null || !questionsWithAnswers.Any())
            {
                return NotFound("No questions found.");
            }
            return Ok(questionsWithAnswers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuestionById(int id)
        {
            var question = await _questionRepository.GetQuestionById(id);
            if (question == null)
            {
                return NotFound($"Question with ID {id} not found.");
            }
            return Ok(question);
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuestion([FromBody] QuestionDto question)
        {
            if (question == null)
            {
                return BadRequest("Question is null.");
            }
            var createResult = await _questionRepository.CreateQuestion(question);
            return Ok(createResult);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateQuestion([FromBody] QuestionDto question)
        {
            var updateResult = await _questionRepository.UpdateQuestion(question);
            return Ok(updateResult);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var deleteResult = await _questionRepository.DeleteQuestion(id);
            return Ok(deleteResult);
        }
    }
}
