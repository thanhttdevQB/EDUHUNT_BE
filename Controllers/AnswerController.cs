using EDUHUNT_BE.DTOs;
using EDUHUNT_BE.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace survey.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnswerController : ControllerBase
    {
        private readonly IAnswer _answerRepository;
        public AnswerController(IAnswer answerRepository)
        {
            _answerRepository = answerRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAnswer()
        {
            var answers = await _answerRepository.GetAnswers();
            if (answers == null || !answers.Any())
            {
                return NotFound("No Answer found.");
            }
            return Ok(answers);
        }

        [HttpGet("{answerId}")]
        public async Task<IActionResult> GetAnswer(int answerId)
        {
            var answer = await _answerRepository.GetAnswerById(answerId);
            if (answer == null)
            {
                return NotFound($"Answer with ID : {answerId} not found");
            }
            if (answer.QuestionId == 0)
            {
                return NotFound("Answer does not have a question");
            }
            return Ok(answer);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAnswer([FromBody] AnswerDto answerCreate)
        {
            if (answerCreate == null)
            {
                return BadRequest("Answer is null.");
            }
            var createResult = await _answerRepository.CreateAnswer(answerCreate);
            return Ok(createResult);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAnswer([FromBody] AnswerDto answer)
        {
            var updateResult = await _answerRepository.UpdateAnswer(answer);
            return Ok(updateResult);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnswer(int id)
        {
            var deleteResult = await _answerRepository.DeleteAnswer(id);
            return Ok(deleteResult);
        }
    }
}