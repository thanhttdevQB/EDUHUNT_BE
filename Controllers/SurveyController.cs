using EDUHUNT_BE.DTOs;
using EDUHUNT_BE.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace survey.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SurveyController : ControllerBase
    {
        private readonly ISurvey _surveyRepository;

        public SurveyController(ISurvey surveyRepository)
        {
            _surveyRepository = surveyRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSurveys()
        {
            var surveys = await _surveyRepository.GetAll();
            if (surveys == null || !surveys.Any())
            {
                return NotFound("No survey found.");
            }
            return Ok(surveys);
        }

        [HttpGet("{surveyId}")]
        public async Task<IActionResult> GetSurveyById(int surveyId)
        {
            var surveyResult = await _surveyRepository.Get(surveyId);

            if (surveyResult == null)
            {
                return NotFound($"Survey with ID : {surveyId} not found");
            }
            return Ok(surveyResult);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetSurveyByUserId(string userId)
        {
            var surveyResult = await _surveyRepository.GetByUserId(userId);

            if (surveyResult == null)
            {
                return NotFound($"Survey with UserID : {userId} not found");
            }
            return Ok(surveyResult);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSurvey([FromBody] SurveyAnswerDto survey)
        {
            if (survey == null)
            {
                return BadRequest("Survey data is null.");
            }
            var result = await _surveyRepository.CreateSurvey(survey);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateSurvey([FromBody] SurveyAnswerDto survey)
        {
            if (survey == null)
            {
                return BadRequest("Survey data is null.");
            }

            var updateResult = await _surveyRepository.UpdateSurvey(survey);
            return Ok(updateResult);
        }

        [HttpDelete("{surveyId}")]
        public async Task<IActionResult> DeleteSurvey(int surveyId)
        {
            try
            {
                var result = await _surveyRepository.Delete(surveyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "An error occurred while deleting the survey.");
            }
        }
    }
}