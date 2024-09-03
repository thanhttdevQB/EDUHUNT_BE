using EDUHUNT_BE.Data;
using EDUHUNT_BE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EDUHUNT_BE.DTOs;
using EDUHUNT_BE.Interfaces;

namespace EDUHUNT_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScholarshipController : ControllerBase

    {
        private readonly AppDbContext _context;
        private readonly IScholarship _scholarshipRepository;

        public ScholarshipController(AppDbContext context, IScholarship scholarshipRepository)
        {
            _context = context;
            _scholarshipRepository = scholarshipRepository;
        }

        [HttpGet("recommended/{userId}")]
        public async Task<IActionResult> GetRecommendedScholarships(string userId)
        {
            try
            {
                var recommendedScholarships = await _scholarshipRepository.GetRecommendedScholarships(userId);

                if (recommendedScholarships == null || !recommendedScholarships.Any())
                {
                    return NotFound("No recommended scholarships found for this user.");
                }

                foreach (var item in recommendedScholarships) {
                    var userScholarships = new UserScholarship
                    {
                        ScholarshipId = item.Id,
                        UserId = userId
                    };
                    _context.UserScholarships.Add(userScholarships);
                }
                await _context.SaveChangesAsync();

                return Ok(recommendedScholarships);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }

        [HttpPost("search")]
        public async Task<IActionResult> SearchScholarships([FromBody] string criteria)
        {
            if (criteria == null || !criteria.Any())
            {
                return BadRequest("Search criteria cannot be null or empty.");
            }

            try
            {
                var scholarships = await _context.ScholarshipInfos
                    .Include(s => s.ScholarshipCategories)
                    .ThenInclude(sc => sc.Category)
                    .Where(s => s.ScholarshipCategories.Any(sc => sc.Category.Name.Contains(criteria)))
                    .Select(s => new ScholarshipDto
                    {
                        Id = s.Id,
                        Title = s.Title,
                        Location = s.Location,
                        School_name = s.SchoolName,
                        Url = s.Url,
                        Level = s.Level,
                        Budget = s.Budget
                    })
                    .ToListAsync();

                if (scholarships == null || !scholarships.Any())
                {
                    return NotFound("No scholarships found matching the search criteria.");
                }

                return Ok(scholarships);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SearchScholarships: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetScholarships()
        {
            try
            {
                var scholarships = await _scholarshipRepository.GetScholarships();
                return Ok(scholarships);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddScholarship([FromBody] ScholarshipDto scholarshipDto)
        {
            try
            {


                // Add scholarship to the database
                await _scholarshipRepository.AddScholarship(scholarshipDto);

                // You can add additional logic if needed, such as returning the added scholarship
                // var addedScholarship = await _scholarshipRepository.GetScholarshipById(scholarshipDto.Id);

                return Ok("Scholarship added successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }


        [HttpDelete("DeleteAll")]
        public async Task<IActionResult> DeleteScholarship()
        {
            try
            {
                await _context.ScholarshipInfos.ExecuteDeleteAsync();
                return Ok("Scholarship deleted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest("Failed to delete resource: "+ex);
            }
        }
    }
}
