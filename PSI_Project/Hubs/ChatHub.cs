using Microsoft.AspNetCore.SignalR;
using PSI_Project.Models;
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
        Comment? addedComment = _chatService.SaveSentMessage(userId, topicId, message);
        if (addedComment is null)
        {
            return;
        }
        
        await Clients.Group(addedComment.TopicId).SendAsync("ReceiveMessage", addedComment.Id, addedComment.CommentText);
    }

    public async Task DeleteMessage(string messageId)
    {
        Comment? deletedMessage = _chatService.DeleteMessage(messageId);
        if (deletedMessage is null)
        {
            return;
        }
        
        await Clients.Group(deletedMessage.TopicId).SendAsync("DeleteMessage", deletedMessage.Id);
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