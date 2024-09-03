using EDUHUNT_BE.Models;
using static EDUHUNT_BE.DTOs.ServiceResponses;

namespace EDUHUNT_BE.Interfaces
{
    public interface IProfiles
    {
        public Task<IEnumerable<Profile>> GetProfile();
        public Task<Profile> GetProfileById(Guid id);
        public Task<GeneralResponse> PutProfile(Guid id, Profile profile);
        public Task<GeneralResponse> PostProfile(Profile profile);
        public Task<GeneralResponse> DeleteProfile(Guid id);

    }
}
