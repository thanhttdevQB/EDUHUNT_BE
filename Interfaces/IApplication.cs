using EDUHUNT_BE.Data;
using EDUHUNT_BE.Models;
using Microsoft.AspNetCore.Identity;
using static EDUHUNT_BE.DTOs.ServiceResponses;

namespace EDUHUNT_BE.Interfaces
{
    public interface IApplication
    {
        public Task<IEnumerable<Application>> GetApplication();
        public Task<Application> GetApplicationById(Guid id, Guid userId);
        public Task<GeneralResponse> PutApplication(Guid id, Application application);
        public Task<GeneralResponse> PostApplication(Application application);
        public Task<GeneralResponse> DeleteApplication(Guid id);
        public Task<IEnumerable<object>> GetApplicationsByScholarshipProvider(string scholarshipProviderId);

    }
}
