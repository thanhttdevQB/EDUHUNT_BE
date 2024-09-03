using EDUHUNT_BE.Data;
using EDUHUNT_BE.DTOs;
using EDUHUNT_BE.Interfaces;
using EDUHUNT_BE.Models;
using Microsoft.EntityFrameworkCore;

namespace EDUHUNT_BE.Repositories
{
    public class MessagesRepository : IMessages
    {
        private readonly AppDbContext _context;
        public MessagesRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<ServiceResponses.GeneralResponse> DeleteMessage(Guid id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return new ServiceResponses.GeneralResponse(false,"Not Found");
            }

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            return new ServiceResponses.GeneralResponse(true,"Delete Successfully");
        }

        public async Task<Message> GetMessageById(Guid id)
        {
            return await _context.Messages.FindAsync(id);
        }

        public async Task<IEnumerable<Message>> GetMessages()
        {
            return await _context.Messages.ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetMessagesByUserId(Guid senderId, Guid receiverId)
        {
            string sender = senderId.ToString();
            string receiver = receiverId.ToString();
            return await _context.Messages.Where(m => m.Sender == sender && m.Receiver == receiver || m.Sender == receiver && m.Receiver == sender).ToListAsync();
        }

        public async Task<ServiceResponses.GeneralResponse> PostMessage(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return new ServiceResponses.GeneralResponse(true, "Post Successfully");
        }

        public async Task<ServiceResponses.GeneralResponse> PutMessage(Guid id, Message message)
        {
            if (id != message.Id)
            {
                return new ServiceResponses.GeneralResponse(false,"Id not found");
            }

            _context.Entry(message).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MessageExists(id))
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

        private bool MessageExists(Guid id)
        {
            return _context.Messages.Any(e => e.Id == id);
        }
    }
}
