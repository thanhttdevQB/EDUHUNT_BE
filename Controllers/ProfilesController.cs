using Microsoft.AspNetCore.Mvc;
using EDUHUNT_BE.Models;
using Microsoft.EntityFrameworkCore;
using EDUHUNT_BE.Data;
using EDUHUNT_BE.Interfaces;

namespace EDUHUNT_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfilesController : ControllerBase
    {
        private readonly IProfiles _profiles;
        private readonly AppDbContext _context;

        public ProfilesController(IProfiles profiles, AppDbContext context)
        {
            _context = context;
            _profiles = profiles;
        }

        // GET: api/Profiles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Profile>>> GetProfile()
        {
            return Ok(await _profiles.GetProfile());
        }

        // GET: api/Profiles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Profile>> GetProfile(Guid id)
        {
            var profile = await _profiles.GetProfileById(id);

            if (profile == null)
            {
                return NotFound();
            }

            return profile;
        }


        // PUT: api/Profiles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProfile(Guid id, Profile profile)
        {
            return Ok(await _profiles.PutProfile(id, profile));
        }

        // POST: api/Profiles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Profile>> PostProfile(Profile profile)
        {
            CreatedAtAction("GetProfile", new { id = profile.Id }, profile);
            return Ok(await _profiles.PostProfile(profile));
        }

        // POST: api/UploadCV
        [HttpPost("UploadCV")]
        public async Task<ActionResult<CV>> UploadCV(CV cV)
        {
            var cv = await _context.CVs.FirstOrDefaultAsync(c => c.UserId == cV.UserId);

            if (string.IsNullOrWhiteSpace(cV.UrlCV))
            {
                if (cv == null)
                {
                    return NotFound("No CV found for the user");
                }
                else
                {
                    return Ok(cv);
                }
            }
            else
            {
                if (cv == null)
                {
                    var newCv = new CV
                    {
                        UserId = cV.UserId,
                        UrlCV = cV.UrlCV
                    };

                    _context.CVs.Add(newCv);
                    await _context.SaveChangesAsync();

                    return CreatedAtAction("GetCV", new { id = newCv.Id }, newCv);
                }
                else
                {
                    cv.UrlCV = cV.UrlCV;
                    _context.Entry(cv).State = EntityState.Modified;

                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!CVExists(cv.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }

                    return NoContent();
                }
            }
        }

        // DELETE: api/Profiles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProfile(Guid id)
        {
            return Ok(await _profiles.DeleteProfile(id));
        }

        private bool CVExists(Guid id)
        {
            return _context.CVs.Any(e => e.Id == id);
        }
    }
}