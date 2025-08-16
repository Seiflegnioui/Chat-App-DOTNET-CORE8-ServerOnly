using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using p4.Models.DTO;
using p4.Models.Entities;
using p4.Services;

namespace p4.Hubs
{
    public class ChatHub(ILogger<ChatHub> log, IConversationService conversationService, IMessageService messageService) : Hub
    {
        public static Dictionary<int, string> ConnectedPeople = new();
        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;

            log.LogInformation("UserID: {UserID}", userId);
            log.LogInformation("ConnectionID: {ConnectionID}", Context.ConnectionId);

            if (!string.IsNullOrEmpty(userId) && int.TryParse(userId, out var ID))
            {
                if (!ConnectedPeople.ContainsKey(ID)) ConnectedPeople.Add(ID, Context.ConnectionId);
                var friendIds = await conversationService.GetFriends(ID);

                var connectedFriends = ConnectedPeople
                    .Where(kvp => friendIds.Contains(kvp.Key))
                    .ToDictionary();

                if (connectedFriends is not null)
                {
                    foreach (var connection in connectedFriends)
                    {
                        await Groups.AddToGroupAsync(connection.Value, $"inbox-{ID}-{connection.Key}");
                        await Groups.AddToGroupAsync(Context.ConnectionId, $"inbox-{ID}-{connection.Key}");
                        await Clients.Caller.SendAsync("getMyConnectionId", $"inbox-{ID}-{connection.Key}");
                    }
                }
            }

            await base.OnConnectedAsync();

        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;

            if (!string.IsNullOrEmpty(userId) &&
                int.TryParse(userId, out var parsedUserId))
            {
                ConnectedPeople.Remove(parsedUserId);
            }

            await base.OnDisconnectedAsync(exception);
        }


        public async Task onConversationStart(string convId)
        {
            try
            {
                var userId = Context.UserIdentifier;

                if (!string.IsNullOrEmpty(userId) &&
                    int.TryParse(userId, out var parsedUserId) &&
                    int.TryParse(convId, out var convParsed))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"conv-{convId}");
                    var updatedMessages = await messageService.MarkAsSeen(convParsed, parsedUserId);
                    await Clients.Group($"conv-{convId}")
                    .SendAsync("SeenUpdate", updatedMessages);
                }
                else
                {
                    log.LogWarning($"Invalid userId '{userId}' or convId '{convId}' when trying to join group.");
                }
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
            await Clients.Group($"conv-{conv}").SendAsync("ReceiveMessage", msg);
            await Clients.Group($"inbox-{msg.SenderId}-{msg.ReceiverId}").SendAsync("InboxReceiveMessage", msg);


        }

        public async Task OnTyping(string conv)
        {
            await Clients.Group($"conv-{conv}").SendAsync("OnTyping");


        }

        public async Task OnMarkSeen(string convId)
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId) &&
                int.TryParse(userId, out var parsedUserId) &&
                int.TryParse(convId, out var convParsed))
            {
                // Ensure current user is in the conversation group
                await Groups.AddToGroupAsync(Context.ConnectionId, $"conv-{convId}");

                var updatedMessages = await messageService.MarkAsSeen(convParsed, parsedUserId);
                log.LogInformation("Updated messages: " + JsonSerializer.Serialize(updatedMessages));
                Console.WriteLine("Updated messages: " + JsonSerializer.Serialize(updatedMessages));


                // notify all participants in the conversation
                await Clients.Group($"conv-{convId}")
                    .SendAsync("SeenUpdate", updatedMessages);

                log.LogInformation($"User {parsedUserId} marked conversation {convId} as seen");
            }
        }



        public async Task StartInboxConnection(int userId)
        {
            var friendIds = await conversationService.GetFriends(userId);

            var connectedFriends = ConnectedPeople
                .Where(kvp => friendIds.Contains(kvp.Key))
                .ToDictionary();

            if (connectedFriends is not null)
            {
                foreach (var connection in connectedFriends)
                {
                    await Groups.AddToGroupAsync(connection.Value, $"inbox-{userId}-{connection.Key}");
                }
            }

        }
    }

}