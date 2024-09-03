using EDUHUNT_BE.DTOs;
using EDUHUNT_BE.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace survey.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentTypeController : ControllerBase
    {
        private readonly IStudentType _studenttypeRepository;

        public StudentTypeController(IStudentType studentTypeRepository)
        {
            _studenttypeRepository = studentTypeRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetStudentType()
        {
            var questions = await _studenttypeRepository.GetStudentType();
            if (questions == null || !questions.Any())
            {
                return NotFound("No questions found.");
            }
            return Ok(questions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudentTypeById(int id)
        {
            var studenttype = await _studenttypeRepository.GetStudentTypeById(id);
            if (studenttype == null)
            {
                return NotFound($"Student Type with ID {id} not found.");
            }
            return Ok(studenttype);
        }

        [HttpPost]
        public async Task<IActionResult> CreateStudentType([FromBody] StudentTypeDto studentType)
        {
            if (studentType == null)
            {
                return BadRequest("Student Type is null.");
            }
            var createResult = await _studenttypeRepository.CreateStudentType(studentType);
            return Ok(createResult);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateStudentType([FromBody] StudentTypeDto studentType)
        {
            var studentTypeResult = await _studenttypeRepository.GetStudentTypeById(studentType.StudentTypeId);
            if (studentType == null || studentTypeResult == null)
            {
                return BadRequest("Student Type is null or ID mismatch.");
            }
            
            var updateResult = await _studenttypeRepository.UpdateStudentType(studentType);
            return Ok(updateResult);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudentType(int id)
        {
            var studentTypeResult = await _studenttypeRepository.DeleteStudentType(id);
            return Ok(studentTypeResult);
        }
    }
}

