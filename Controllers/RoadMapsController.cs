using Microsoft.AspNetCore.Mvc;
using EDUHUNT_BE.Models;
using EDUHUNT_BE.Interfaces;

namespace EDUHUNT_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoadMapsController : ControllerBase
    {
        private readonly IRoadMap _roadMap;

        public RoadMapsController(IRoadMap roadMap)
        {
            _roadMap = roadMap;
        }

        // GET: api/RoadMaps
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoadMap>>> GetRoadMap()
        {
            return Ok(await _roadMap.GetRoadMap());
        }

        // GET: api/RoadMaps/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<RoadMap>>> GetRoadMapById(string id)
        {
            var roadMaps = await _roadMap.GetRoadMapById(id);

            if (roadMaps == null)
            {
                return NotFound();
            }

            return Ok(roadMaps);
        }

        // PUT: api/roadmaps/5/approve
        [HttpPut("{id}/approve")]
        public async Task<IActionResult> ApproveRoadMap(Guid id, [FromBody] bool isApproved)
        {
            return Ok(await _roadMap.ApproveRoadMap(id, isApproved));
        }

        // POST: api/RoadMaps
        [HttpPost]
        public async Task<ActionResult<List<RoadMap>>> PostRoadMaps(List<RoadMap> roadMaps)
        {
            CreatedAtAction("GetRoadMap", roadMaps);
            return Ok(await _roadMap.PostRoadMap(roadMaps));
        }

        // DELETE: api/RoadMaps/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoadMap(Guid id)
        {
            return Ok(await _roadMap.DeleteRoadMap(id));
        }
    }
}