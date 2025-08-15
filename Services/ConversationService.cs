using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using p4.Data;
using p4.Models.DTO;
using p4.Models.Entities;

namespace p4.Services
{
    public class ConversationService(IConfiguration config, AppDbContext context, IHttpContextAccessor http, ILogger<ConversationService> logger) : IConversationService
    {
        public async Task<int> StartAsyn(ConvDTO conv)
        {
            try
            {
                var user = http.HttpContext?.User;
                var userIdStr = user?.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userIdStr))
                {
                    logger.LogWarning("Unauthorized access attempt.");
                    throw new UnauthorizedAccessException("User not authenticated.");
                }

                var senderId = int.Parse(userIdStr);
                logger.LogInformation("Sender ID: {SenderId}", senderId);

                var prev_conv = await context.Conversations.FirstOrDefaultAsync(c => (c.ReceiverId == conv.ReceiverId && c.SenderId == senderId) || (c.SenderId == conv.ReceiverId && c.ReceiverId == senderId));
                if (prev_conv != null)
                {
                    return prev_conv.Id;
                }
                var conversation = new Conversation
                {
                    SenderId = senderId,
                    ReceiverId = conv.ReceiverId,
                    last_join = conv.last_join
                };
                var new_conv = await context.Conversations.AddAsync(conversation);
                await context.SaveChangesAsync();
                return conversation.Id;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while starting a conversation.");
                throw;
            }
        }

      public async Task<List<int>> GetFriends(int myid)
        {
            var conversations = await context.Conversations
                .Where(c => c.ReceiverId == myid || c.SenderId == myid)
                .Select(c => new
                {
                    c.SenderId,
                    c.ReceiverId
                })
                .ToListAsync();

            var friendIds = new List<int>();

            foreach (var row in conversations)
            {
                if (row.SenderId == myid)
                    friendIds.Add(row.ReceiverId); 
                else
                    friendIds.Add(row.SenderId); 
            }
            friendIds = friendIds.Distinct().ToList();
            friendIds.ForEach((f =>
            {
                logger.LogInformation("Freind : " + f.ToString());
            })) ;

            return friendIds;
        }

    }
}
