using EDUHUNT_BE.Data;
using EDUHUNT_BE.Models;
using static EDUHUNT_BE.DTOs.ServiceResponses;

namespace EDUHUNT_BE.Interfaces
{
    public interface IQas
    {
        public Task<IEnumerable<QA>> GetQAs();
        public Task<IEnumerable<ApplicationUser>> GetUsersOrMentors(Guid id);
        public Task<IEnumerable<QA>> GetConversations(Guid id);
        public Task<IEnumerable<QA>> GetQAsByUserId(Guid answerId, Guid askedId);
        public Task<GeneralResponse> PutQA(Guid id, QA qA);
        public Task<QA> GetQADetail(Guid id);
        public Task<GeneralResponse> PostQA(QA qA);
        public Task<GeneralResponse> DeleteQA(Guid id);
    }
}
