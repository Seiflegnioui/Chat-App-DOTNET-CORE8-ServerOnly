using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using p4.Data;
using p4.Models.DTO;
using p4.Models.Entities;

namespace p4.Services
{
    public class MessageService(IHttpContextAccessor http, AppDbContext context,ILogger<MessageService> log) : IMessageService
    {
        public async Task<List<Msg>> All(int conv)
        {
            var messages = await context.Msgs.Where(m => m.ConversationID == conv).Include(c=>c.Sender)
            .Include(c=>c.Receiver).OrderBy(m=> m.time).ToListAsync();
            return messages;
        }

        
       

        public async Task<Msg> Send(MesssageDTO message)
        {
            var userIdString = http.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString))
                throw new Exception("User is not authenticated.");

            var senderId = int.Parse(userIdString);

            var myConv = await context.Conversations
                .SingleOrDefaultAsync(c => c.Id == message.ConversationId);

            if (myConv == null)
                throw new Exception("Conversation not found.");

            int receiverId = (senderId == myConv.SenderId) ? myConv.ReceiverId : myConv.SenderId;

            var msg = new Msg
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                ConversationID = message.ConversationId,
                content = message.content,
                time = DateTime.Now
            };

            var m = await context.Msgs.AddAsync(msg);
            await context.SaveChangesAsync();
            return m.Entity;
        }
}

}
