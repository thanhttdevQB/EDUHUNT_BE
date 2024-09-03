using EDUHUNT_BE.Data;
using EDUHUNT_BE.DTOs;
using EDUHUNT_BE.Interfaces;
using EDUHUNT_BE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EDUHUNT_BE.Repositories
{
    public class ScholarshipInfoesRepository : IScholarshipInfoes
    {
        private readonly AppDbContext _context;
        public ScholarshipInfoesRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponses.GeneralResponse> ApproveScholarship(Guid id, [FromBody] bool isApproved)
        {
            var scholarship = await _context.ScholarshipInfos.FindAsync(id);
            if (scholarship == null)
            {
                return new ServiceResponses.GeneralResponse(false,"Not Found");
            }

            if (isApproved)
            {
                scholarship.IsApproved = true;
                _context.Entry(scholarship).State = EntityState.Modified;
            }
            else
            {
                _context.ScholarshipInfos.Remove(scholarship);
            }
            await _context.SaveChangesAsync();

            return new ServiceResponses.GeneralResponse(true,"Approve Successfully");
        }

        public async Task<ServiceResponses.GeneralResponse> DeleteScholarshipInfo(Guid id)
        {
            var scholarshipInfo = await _context.ScholarshipInfos.FindAsync(id);
            if (scholarshipInfo == null)
            {
                return new ServiceResponses.GeneralResponse(false,"Not Found");
            }

            _context.ScholarshipInfos.Remove(scholarshipInfo);
            await _context.SaveChangesAsync();

            return new ServiceResponses.GeneralResponse(true,"Delete Successfully");
        }

        public async Task<IEnumerable<ScholarshipInfo>> GetScholarshipInfo()
        {
            return await _context.ScholarshipInfos.ToListAsync();
        }

        public async Task<ScholarshipInfo> GetScholarshipInfoById(Guid id)
        {
            return await _context.ScholarshipInfos.FindAsync(id);
        }

        public async Task<IEnumerable<UserScholarship>> GetUserScholarshipInfo()
        {
            return await _context.UserScholarships.ToListAsync();
        }

        public async Task<IEnumerable<ScholarshipDto>> GetUserScholarshipInfoByUserId(string userId)
        {
            var userScholarships = await _context.UserScholarships
                .Where(us => us.UserId == userId)
                .Join(_context.ScholarshipInfos,
                      us => us.ScholarshipId,
                      si => si.Id,
                      (us, si) => new { UserScholarship = us, ScholarshipInfo = si })
                .ToListAsync();

            var scholarshipDtos = userScholarships.Select(us => new ScholarshipDto
            {
                Id = us.ScholarshipInfo.Id,
                Title = us.ScholarshipInfo.Title,
                Budget = us.ScholarshipInfo.Budget,
                Location = us.ScholarshipInfo.Location,
                School_name = us.ScholarshipInfo.SchoolName,
                Level = us.ScholarshipInfo.Level,
                Url = us.ScholarshipInfo.Url
            });

            return scholarshipDtos;
        }

        public async Task<ServiceResponses.GeneralResponse> PostScholarshipInfo(ScholarshipInfoDto scholarshipInfo)
        {
            scholarshipInfo.Id = Guid.NewGuid();
            scholarshipInfo.IsInSite = true;
            var scholarship = new ScholarshipInfo
            {
                Id = scholarshipInfo.Id,
                Title = scholarshipInfo.Title,
                Budget = scholarshipInfo.Budget,
                Location = scholarshipInfo.Location,
                SchoolName = scholarshipInfo.SchoolName,
                Level = scholarshipInfo.Level,
                Url = scholarshipInfo.Url,
                IsApproved = false,
                IsInSite = scholarshipInfo.IsInSite,
                ImageUrl = scholarshipInfo.ImageUrl,
                Description = scholarshipInfo.Description,
                CreatedAt = DateTime.UtcNow,
                AuthorId = scholarshipInfo.AuthorId,
                ScholarshipCategories = [],
            };
            _context.ScholarshipInfos.Add(scholarship);

            await _context.SaveChangesAsync();

            return new ServiceResponses.GeneralResponse(true, "Post Successfully");
        }

        public async Task<ServiceResponses.GeneralResponse> PutScholarshipInfo(Guid id, ScholarshipInfo scholarshipInfo)
        {
            if (id != scholarshipInfo.Id)
            {
                return new ServiceResponses.GeneralResponse(false,"Id not found");
            }

            _context.Entry(scholarshipInfo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ScholarshipInfoExists(id))
                {
                    return new ServiceResponses.GeneralResponse(false,"Not Found");
                }
                else
                {
                    throw;
                }
            }

            return new ServiceResponses.GeneralResponse(true,"Put Successfully");
        }

        private bool ScholarshipInfoExists(Guid id)
        {
            return _context.ScholarshipInfos.Any(e => e.Id == id);
        }
    }
}
