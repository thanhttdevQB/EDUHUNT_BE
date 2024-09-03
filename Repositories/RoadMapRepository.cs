using EDUHUNT_BE.Data;
using EDUHUNT_BE.DTOs;
using EDUHUNT_BE.Interfaces;
using EDUHUNT_BE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EDUHUNT_BE.Repositories
{
    public class RoadMapRepository : IRoadMap
    {

        private readonly AppDbContext _context;
        public RoadMapRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponses.GeneralResponse> ApproveRoadMap(Guid id, [FromBody] bool isApproved)
        {
            var roadMap = await _context.RoadMaps.FindAsync(id);
            if (roadMap == null)
            {
                return new ServiceResponses.GeneralResponse(false, "Not Found");
            }

            if (isApproved)
            {
                roadMap.IsApproved = true;
                _context.Entry(roadMap).State = EntityState.Modified;
            }
            else
            {
                _context.RoadMaps.Remove(roadMap);
            }
            await _context.SaveChangesAsync();

            return new ServiceResponses.GeneralResponse(true,"Approved Successfully");
        }

        public async Task<ServiceResponses.GeneralResponse> DeleteRoadMap(Guid id)
        {
            var roadMap = await _context.RoadMaps.FindAsync(id);
            if (roadMap == null)
            {
                return new ServiceResponses.GeneralResponse(false,"Not Found");
            }

            _context.RoadMaps.Remove(roadMap);
            await _context.SaveChangesAsync();

            return new ServiceResponses.GeneralResponse(true,"Delete Successfully");
        }

        public async Task<IEnumerable<RoadMap>> GetRoadMap()
        {
            return await _context.RoadMaps.ToListAsync();
        }

        public async Task<IEnumerable<RoadMap>> GetRoadMapById(string id)
        {
            return await _context.RoadMaps.Where(r => r.UserId == id).ToListAsync();

        }

        public async Task<ServiceResponses.GeneralResponse> PostRoadMap(List<RoadMap> roadMaps)
        {
            foreach (var roadMap in roadMaps)
            {
                roadMap.Id = Guid.NewGuid();
                _context.RoadMaps.Add(roadMap);
            }

            await _context.SaveChangesAsync();

            return new ServiceResponses.GeneralResponse(true,"Post Successfully");
        }
    }
}
