using Microsoft.AspNetCore.Mvc;
using EDUHUNT_BE.Data;
using EDUHUNT_BE.Models;
using EDUHUNT_BE.Interfaces;

namespace EDUHUNT_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QAsController : ControllerBase
    {
        private readonly IQas _qas;

        public QAsController(IQas qas)
        {
            _qas = qas;
        }

        // GET: api/QAs
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<QA>>> GetQAs()
        {
            return Ok(await _qas.GetQAs());
        }

        // GET: api/QAs/GetAllUserOrMentor/{id}
        [HttpGet("GetAllUserOrMentor/{id}")]
        public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetUsersOrMentors(Guid id)
        {
            return Ok(await _qas.GetUsersOrMentors(id));
        }

        // GET: api/QAs/Conversations/{id}
        [HttpGet("Conversations/{id}")]
        public async Task<ActionResult<IEnumerable<QA>>> GetConversations(string id)
        {
            if (!Guid.TryParse(id, out Guid userId))
            {
                return BadRequest("Invalid user ID format");
            }

            return Ok(await _qas.GetConversations(userId));
        }


        // GET: api/QAs/ByUserId/
        [HttpGet("ByUserId")]
        public async Task<ActionResult<IEnumerable<QA>>> GetQAsByUserId(Guid answerId, Guid askedId)
        {
            var qAsByUser = await _qas.GetQAsByUserId(answerId, askedId);
            if (qAsByUser == null || !qAsByUser.Any())
            {
                return NotFound();
            }
            return Ok(qAsByUser);
        }


        // GET: api/QAs/Detail/{id}
        [HttpGet("Detail/{id}")]
        public async Task<ActionResult<QA>> GetQADetail(Guid id)
        {
            return Ok(await _qas.GetQADetail(id));
        }

        // PUT: api/QAs/Edit/{id}
        [HttpPut("Edit/{id}")]
        public async Task<IActionResult> PutQA(Guid id, QA qA)
        {
            return Ok(await _qas.PutQA(id, qA));
        }

        // POST: api/QAs/Create
        [HttpPost("Create")]
        public async Task<ActionResult<QA>> PostQA(QA qA)
        {
            CreatedAtAction("GetQA", new { id = qA.Id }, qA);
            return Ok(await _qas.PostQA(qA));
        }

        // DELETE: api/QAs/Delete/{id}
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteQA(Guid id)
        {
            return Ok(await _qas.DeleteQA(id)); 
        }
    }
}
