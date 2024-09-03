using EDUHUNT_BE.Data;
using EDUHUNT_BE.DTOs;
using EDUHUNT_BE.Interfaces;
using EDUHUNT_BE.Models;
using Microsoft.EntityFrameworkCore;
using static EDUHUNT_BE.DTOs.ServiceResponses;

namespace EDUHUNT_BE.Repositories
{
    public class ApplicationRepository : IApplication
    {
        public AppDbContext _context { get; set; }
        public ApplicationRepository(AppDbContext context) {
            _context = context;
        }
        public async Task<GeneralResponse> DeleteApplication(Guid id)
        {
            var application = await _context.Applications.FindAsync(id);
            if (application == null)
            {
                return new GeneralResponse(false,"Not found application");
            }

            _context.Applications.Remove(application);
            await _context.SaveChangesAsync();

            return new GeneralResponse(true,"Delete Success");


        }

        public async Task<IEnumerable<Application>> GetApplication()
        {
            return await _context.Applications.ToListAsync();
        }

        public async Task<Application> GetApplicationById(Guid id, Guid userId)
        {
            var application = await _context.Applications.Where(x=>((x.ScholarshipID==id&&x.StudentID==userId)||x.Id==id).Equals(true)).SingleOrDefaultAsync();
            return application;
        }

        public async Task<IEnumerable<object>> GetApplicationsByScholarshipProvider(string scholarshipProviderId)
        {
            var applicationsWithScholarshipInfo = await _context.Applications
                .Join(_context.ScholarshipInfos, // The table we are joining with
                      application => application.ScholarshipID, // Foreign key in Applications
                      scholarshipInfo => scholarshipInfo.Id, // Primary key in ScholarshipInfo
                      (application, scholarshipInfo) => new { Application = application, ScholarshipInfo = scholarshipInfo }) // Result selector
                .Where(result => result.ScholarshipInfo.AuthorId == scholarshipProviderId) // Filter by ScholarshipProviderId
                .Select(result => new
                {
                    // Application details
                    ApplicationId = result.Application.Id,
                    StudentID = result.Application.StudentID,
                    ScholarshipID = result.Application.ScholarshipID,
                    StudentCV = result.Application.StudentCV,
                    Status = result.Application.Status,
                    MeetingURL = result.Application.MeetingURL,
                    StudentChooseDay = result.Application.StudentChooseDay,
                    ScholarshipProviderAvailableStartDate = result.Application.ScholarshipProviderAvailableStartDate,
                    ScholarshipProviderAvailableEndDate = result.Application.ScholarshipProviderAvailableEndDate,
                    ApplicationReason = result.Application.ApplicationReason,

                    // ScholarshipInfo details
                    ScholarshipTitle = result.ScholarshipInfo.Title,
                    ScholarshipBudget = result.ScholarshipInfo.Budget,
                    ScholarshipLocation = result.ScholarshipInfo.Location,
                    SchoolName = result.ScholarshipInfo.SchoolName,
                    Description = result.ScholarshipInfo.Description,
                    IsInSite = result.ScholarshipInfo.IsInSite,
                    Url = result.ScholarshipInfo.Url,
                    CreatedAt = result.ScholarshipInfo.CreatedAt,
                    IsApproved = result.ScholarshipInfo.IsApproved,
                    ImageUrl = result.ScholarshipInfo.ImageUrl
                })
                .ToListAsync();

            return applicationsWithScholarshipInfo;
        }

        public async Task<ServiceResponses.GeneralResponse> PostApplication(Application application)
        {
            _context.Applications.Add(application);
            await _context.SaveChangesAsync();
            return new GeneralResponse(true, "Post successfully");
        }

        public async Task<ServiceResponses.GeneralResponse> PutApplication(Guid id, Application application)
        {
            if (id != application.Id)
            {
                return new GeneralResponse(false, "Id not exist");
            }

            _context.Entry(application).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApplicationExists(id))
                {
                    return new GeneralResponse(false,"Not Found");
                }
                else
                {
                    throw;
                }
            }

            return new GeneralResponse(true,"Put successfully");
        }

        private bool ApplicationExists(Guid id)
        {
            return _context.Applications.Any(e => e.Id == id);
        }
    }
}
