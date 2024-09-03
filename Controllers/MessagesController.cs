using Microsoft.AspNetCore.Mvc;
using EDUHUNT_BE.Models;
using EDUHUNT_BE.Interfaces;

namespace EDUHUNT_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessages _messages;

        public MessagesController(IMessages messages)
        {
            _messages = messages;
        }

        // GET: api/Messages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessages()
        {
            return Ok(await _messages.GetMessages());
        }

        // GET: api/Messages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Message>> GetMessage(Guid id)
        {
            var message = await _messages.GetMessageById(id);

            if (message == null)
            {
                return NotFound();
            }

            return message;
        }

        // PUT: api/Messages/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMessage(Guid id, Message message)
        {
            return Ok(await _messages.PutMessage(id, message));
        }

        // POST: api/Messages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Message>> PostMessage(Message message)
        {
            CreatedAtAction("GetMessage", new { id = message.Id }, message);
            return Ok(await _messages.PostMessage(message));
        }

        // DELETE: api/Messages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(Guid id)
        {
            return Ok(await _messages.DeleteMessage(id));
        }

        // GET: api/Messages/user/id
        [HttpGet("user/{senderId}/{receiverId}")]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessagesByUserId(Guid senderId, Guid receiverId)
        {
            var messages = await _messages.GetMessagesByUserId(senderId,receiverId);

            if (messages == null)
            {
                return NotFound();
            }

            return Ok(messages);
        }
    }
}
