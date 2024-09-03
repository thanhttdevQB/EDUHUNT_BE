using EDUHUNT_BE.DTOs;
using EDUHUNT_BE.Models;
using Microsoft.AspNetCore.Mvc;
using static EDUHUNT_BE.DTOs.ServiceResponses;

namespace EDUHUNT_BE.Interfaces
{
    public interface IScholarshipInfoes
    {
        public Task<IEnumerable<ScholarshipInfo>> GetScholarshipInfo();
        public Task<IEnumerable<UserScholarship>> GetUserScholarshipInfo();
        public Task<IEnumerable<ScholarshipDto>> GetUserScholarshipInfoByUserId(string userId);
        public Task<ScholarshipInfo> GetScholarshipInfoById(Guid id);
        public Task<GeneralResponse> PutScholarshipInfo(Guid id, ScholarshipInfo scholarshipInfo);
        public Task<GeneralResponse> PostScholarshipInfo(ScholarshipInfoDto scholarshipInfo);
        public Task<GeneralResponse> DeleteScholarshipInfo(Guid id);
        public Task<GeneralResponse> ApproveScholarship(Guid id, [FromBody] bool isApproved);
    }
}
