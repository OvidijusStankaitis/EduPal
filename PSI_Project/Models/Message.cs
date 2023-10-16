namespace PSI_Project.Models;

public class Message : BaseEntity
{
    public string Text { get; set; }
    public bool IsUserMessage { get; set; }

    public Message(string text, bool isUserMessage) : base("Message")
    {
        Text = text;
        IsUserMessage = isUserMessage;
    }
}