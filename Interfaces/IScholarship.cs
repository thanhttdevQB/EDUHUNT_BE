using EDUHUNT_BE.DTOs;
using EDUHUNT_BE.Models;
using Microsoft.AspNetCore.Mvc;

namespace EDUHUNT_BE.Interfaces
{
    public interface IScholarship
    {
        Task<List<ScholarshipDto>> GetScholarships();
        Task AddScholarship(ScholarshipDto scholarship);
        Task<List<ScholarshipInfo>> GetRecommendedScholarships(string userId);
    }
}
