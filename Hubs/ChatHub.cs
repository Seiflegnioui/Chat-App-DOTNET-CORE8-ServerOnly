using Microsoft.AspNetCore.SignalR;
using p4.Models.DTO;
using p4.Models.Entities;

namespace p4.Hubs
{
    public class ChatHub(ILogger<ChatHub> log) : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            log.LogInformation("UserID: {UserID}", userId);
            log.LogInformation("ConnectionID: {ConnectionID}", Context.ConnectionId);

            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
                await Clients.Caller.SendAsync("getMyConnectionId", Context.ConnectionId);
            }

            await base.OnConnectedAsync();
        }

        public async Task onConversationStart(string convId)
        {
            try
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"conv-{convId}");
                log.LogInformation($"User is on group {convId}");
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Failed to join group");
            }

        }

        public async Task onConversationLeave(string convId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"conv-{convId}");
            log.LogInformation($"user is off group {convId}");
        }

        public async Task sendMessage(string conv, Msg msg)
        {
            await Clients.Group($"conv-{conv}").SendAsync("ReceiveMessage",msg);
        }
    }

}