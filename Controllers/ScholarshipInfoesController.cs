using Microsoft.AspNetCore.Mvc;
using EDUHUNT_BE.Models;
using EDUHUNT_BE.Interfaces;
using EDUHUNT_BE.DTOs;

namespace EDUHUNT_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScholarshipInfoesController : ControllerBase
    {
        private readonly IScholarshipInfoes _scholarshipInfoes;

        public ScholarshipInfoesController(IScholarshipInfoes scholarshipInfoes)
        {
            _scholarshipInfoes = scholarshipInfoes;
        }

        // GET: api/ScholarshipInfoes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ScholarshipInfo>>> GetScholarshipInfos()
        {
            return Ok(await _scholarshipInfoes.GetScholarshipInfo());
        }

        [HttpGet("recommend")]
        public async Task<ActionResult<IEnumerable<UserScholarship>>> GetUserScholarshipInfos()
        {
            return Ok(await _scholarshipInfoes.GetUserScholarshipInfo());
        }

        [HttpGet("recommend/{userId}")]
        public async Task<ActionResult<IEnumerable<ScholarshipDto>>> GetUserScholarshipInfosByUserId(string userId)
        {
            return Ok(await _scholarshipInfoes.GetUserScholarshipInfoByUserId(userId));
        }

        // GET: api/ScholarshipInfoes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ScholarshipInfo>> GetScholarshipInfo(Guid id)
        {
            var scholarshipInfo = await _scholarshipInfoes.GetScholarshipInfoById(id);

            if (scholarshipInfo == null)
            {
                return NotFound();
            }

            return scholarshipInfo;
        }

        // PUT: api/ScholarshipInfoes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutScholarshipInfo(Guid id, ScholarshipInfo scholarshipInfo)
        {
            return Ok(await _scholarshipInfoes.PutScholarshipInfo(id,scholarshipInfo));
        }

        // PUT: api/ScholarshipInfoes/5/approve
        [HttpPut("{id}/approve")]
        public async Task<IActionResult> ApproveScholarship(Guid id, [FromBody] bool isApproved)
        {
            return Ok(await _scholarshipInfoes.ApproveScholarship(id, isApproved));
        }

        // POST: api/ScholarshipInfoes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ScholarshipInfo>> PostScholarshipInfo(ScholarshipInfoDto scholarshipInfo)
        {
            CreatedAtAction("GetScholarshipInfo", new { id = scholarshipInfo.Id }, scholarshipInfo);
            return Ok(await _scholarshipInfoes.PostScholarshipInfo(scholarshipInfo));
        }

        // DELETE: api/ScholarshipInfoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteScholarshipInfo(Guid id)
        {
            return Ok(await _scholarshipInfoes.DeleteScholarshipInfo(id));
        }
    }
}
