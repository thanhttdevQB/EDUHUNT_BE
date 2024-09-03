using Microsoft.AspNetCore.Mvc;
using EDUHUNT_BE.Models;
using EDUHUNT_BE.Interfaces;

namespace EDUHUNT_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationsController : ControllerBase
    {
        private readonly IApplication _application;

        public ApplicationsController(IApplication application)
        {
            _application = application;
        }

        // GET: api/Applications
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Application>>> GetApplications()
        {
            return Ok(await _application.GetApplication());
        }

        // GET: api/Applications/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Application>> GetApplication(Guid id, Guid userId)
        {
            var application = await _application.GetApplicationById(id, userId);

            if (application == null)
            {
                return NotFound();
            }

            return application;
        }

        // PUT: api/Applications/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutApplication(Guid id, Application application)
        {
            return Ok(await _application.PutApplication(id, application));
        }

        // POST: api/Applications
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Application>> PostApplication(Application application)
        {
            CreatedAtAction("GetApplication", new { id = application.Id }, application);
            return Ok (await _application.PostApplication(application));
        }

        // DELETE: api/Applications/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApplication(Guid id)
        {
            var response = await _application.DeleteApplication(id);
            return Ok(response);
        }


        // GET: api/Applications/ScholarshipProvider/{scholarshipProviderId}
        [HttpGet("ScholarshipProvider/{scholarshipProviderId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetApplicationsByScholarshipProvider(string scholarshipProviderId)
        {
            var applicationsWithScholarshipInfo = await _application.GetApplicationsByScholarshipProvider(scholarshipProviderId);

            if (!applicationsWithScholarshipInfo.Any())
            {
                return NotFound();
            }

            return Ok(applicationsWithScholarshipInfo);
        }
    }
}