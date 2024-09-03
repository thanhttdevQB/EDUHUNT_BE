using EDUHUNT_BE.Models;
using Microsoft.AspNetCore.Mvc;
using static EDUHUNT_BE.DTOs.ServiceResponses;

namespace EDUHUNT_BE.Interfaces
{
    public interface IRoadMap
    {
        public Task<IEnumerable<RoadMap>> GetRoadMap();
        public Task<IEnumerable<RoadMap>> GetRoadMapById(string id);
        public Task<GeneralResponse> ApproveRoadMap(Guid id, [FromBody] bool isApproved);
        public Task<GeneralResponse> PostRoadMap(List<RoadMap> roadMaps);
        public Task<GeneralResponse> DeleteRoadMap(Guid id);
    }
}
