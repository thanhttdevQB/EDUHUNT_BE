using EDUHUNT_BE.Data;
using EDUHUNT_BE.DTOs;
using EDUHUNT_BE.Interfaces;
using EDUHUNT_BE.Models;
using Microsoft.EntityFrameworkCore;

namespace EDUHUNT_BE.Repositories
{
    public class ProfileRepository : IProfiles
    {
        private readonly AppDbContext _context;

        public ProfileRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponses.GeneralResponse> DeleteProfile(Guid id)
        {
            var profile = await _context.Profile.FindAsync(id);
            if (profile == null)
            {
                return new ServiceResponses.GeneralResponse(false, "Not Found");
            }

            _context.Profile.Remove(profile);
            await _context.SaveChangesAsync();

            return new ServiceResponses.GeneralResponse(true,"Delete Successfully");
        }

        public async Task<IEnumerable<Profile>> GetProfile()
        {
            return await _context.Profile.ToListAsync();
        }

        public async Task<Profile> GetProfileById(Guid id)
        {
            return await _context.Profile.FirstOrDefaultAsync(p => p.UserId == id);
        }

        public async Task<ServiceResponses.GeneralResponse> PostProfile(Profile profile)
        {
            _context.Profile.Add(profile);
            await _context.SaveChangesAsync();

            return new ServiceResponses.GeneralResponse(true, "Post Successfully");
        }

        public async Task<ServiceResponses.GeneralResponse> PutProfile(Guid id, Profile profile)
        {
            if (id != profile.Id)
            {
                return new ServiceResponses.GeneralResponse(false, "Id not found");
            }
            var currentProfile = await _context.Profile.FirstOrDefaultAsync(p => p.Id == id);

            if (currentProfile.ContentURL != profile.ContentURL)
            {
                currentProfile.ContentURL = profile.ContentURL;
            }
            if (currentProfile.FirstName != profile.FirstName)
            {
                currentProfile.FirstName = profile.FirstName;
            }
            if (currentProfile.LastName != profile.LastName)
            {
                currentProfile.LastName = profile.LastName;
            }
            if (currentProfile.UserName != profile.UserName)
            {
                currentProfile.UserName = profile.UserName;
            }
            if (currentProfile.ContactNumber != profile.ContactNumber)
            {
                currentProfile.ContactNumber = profile.ContactNumber;
            }
            if (currentProfile.Address != profile.Address)
            {
                currentProfile.Address = profile.Address;
            }
            if (currentProfile.Description != profile.Description)
            {
                currentProfile.Description = profile.Description;
            }
            if (currentProfile.UrlAvatar != profile.UrlAvatar)
            {
                currentProfile.UrlAvatar = profile.UrlAvatar;
            }
            if (currentProfile.IsVIP != profile.IsVIP)
            {
                currentProfile.IsVIP = profile.IsVIP;
            }
            try
            {

                _context.Entry(currentProfile).State = EntityState.Modified;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProfileExists(id))
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

        private bool ProfileExists(Guid id)
        {
            return _context.Profile.Any(e => e.Id == id);
        }
    }
}
