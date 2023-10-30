using Microsoft.AspNetCore.SignalR;
using PSI_Project.Services;

namespace PSI_Project.Hubs;

public class ChatHub : Hub
{
    private readonly ChatService _chatService; 
    
    public ChatHub(ChatService chatService)
    {
        _chatService = chatService;
    }

    public async Task SendMessage(string userId, string topicId, string message)
    {
        _chatService.SaveSentMessage(userId, topicId, message);
        await Clients.OthersInGroup(topicId).SendAsync("ReceiveMessage", message);    // invokes response in client
    }

    public async Task AddToBroadcastGroup(string topicId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, topicId);
    }
    
    public async Task RemoveFromBroadcastGroup(string topicId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, topicId);
    }
}