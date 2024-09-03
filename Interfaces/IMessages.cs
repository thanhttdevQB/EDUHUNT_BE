using EDUHUNT_BE.Models;
using static EDUHUNT_BE.DTOs.ServiceResponses;

namespace EDUHUNT_BE.Interfaces
{
    public interface IMessages
    {
        public Task<IEnumerable<Message>> GetMessages();
        public Task<Message> GetMessageById(Guid id);
        public Task<GeneralResponse> PutMessage(Guid id, Message message);
        public Task<GeneralResponse> PostMessage(Message message);
        public Task<GeneralResponse> DeleteMessage(Guid id);
        public Task<IEnumerable<Message>> GetMessagesByUserId(Guid senderId, Guid receiverId);
    }
}
