using Microsoft.AspNetCore.SignalR;
using PSI_Project.Models;
using PSI_Project.Services;

namespace PSI_Project.Hubs;

public class ChatHub : Hub
{
    private readonly IUserAuthService _userAuthService;
    private readonly ChatService _chatService;
    private readonly ILogger<ChatHub> _logger;
    
    public ChatHub(ChatService chatService, IUserAuthService userAuthService, ILogger<ChatHub> logger)
    {
        _userAuthService = userAuthService;
        _chatService = chatService;
        _logger = logger;
    }
    
    public async Task SendMessage(string topicId, string message)
    {
        try
        {
            HttpContext? context = Context.GetHttpContext();
            User? user = await _userAuthService.GetUser(context!);            
            Comment? addedComment = _chatService.SaveSentMessage(user!.Id, topicId, message);
            await Clients.OthersInGroup(addedComment!.TopicId).SendAsync("ReceiveMessage", addedComment.Id, addedComment.Content, addedComment.Timestamp, false);
            await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage", addedComment.Id, addedComment.Content,addedComment.Timestamp, true);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error while sending message (messageID: {message}) in topic - {topicId}", message, topicId);
        }
    }

    public async Task DeleteMessage(string messageId)
    {
        try
        {
            Comment deletedMessage = await _chatService.DeleteMessage(messageId);
            await Clients.Group(deletedMessage.TopicId).SendAsync("DeleteMessage", deletedMessage.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while deleting message {messageId}", messageId);
        }
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