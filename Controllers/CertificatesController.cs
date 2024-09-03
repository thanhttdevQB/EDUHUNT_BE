using Microsoft.AspNetCore.Mvc;
using EDUHUNT_BE.Models;
using EDUHUNT_BE.Interfaces;

namespace EDUHUNT_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertificatesController : ControllerBase
    {
        private readonly ICertificate _certificate;

        public CertificatesController(ICertificate certificate)
        {
            _certificate = certificate;
        }

        // GET: api/certificates
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Certificate>>> GetCertificate()
        {
            return Ok(await _certificate.GetCertificates());
        }

        // GET: api/certificates/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Certificate>> GetCertificateById(Guid id)
        {
            var certificate = await _certificate.GetCertificateById(id);

            if (certificate == null)
            {
                return NotFound();
            }

            return certificate;
        }

        // PUT: api/Certificates/5/approve
        [HttpPut("{id}/approve")]
        public async Task<IActionResult> ApproveCertificate(Guid id, [FromBody] bool isApproved)
        {
            return Ok(await _certificate.ApproveCertificate(id, isApproved));
        }

        // POST: api/certificates
        [HttpPost]
        public async Task<ActionResult<Certificate>> PostCertificate(Certificate certificate)
        {
            CreatedAtAction("Getcertificate", new { id = certificate.Id }, certificate);
            return Ok(await _certificate.PostCertificate(certificate));
        }

        // DELETE: api/certificates/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCertificate(Guid id)
        {
            return Ok(await _certificate.DeleteCertificate(id));
        }
    }
}