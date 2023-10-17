using PSI_Project.Models;

namespace PSI_Project.Repositories;

public class OpenAIRepository : BaseRepository<Message>
{
    protected override string DbFilePath => "..//PSI_Project//DB//openai.txt";

    protected override string ItemToDbString(Message item)
    {
        return $"{item.Id};{item.Text};{item.Email};{(item.IsUserMessage ? 1 : 0)}";
    }

    protected override Message StringToItem(string dbString)
    {
        var fields = dbString.Split(';');
        var sender = fields[3] == "1" ? "user" : "chatgpt";
        var message = new Message(fields[1], fields[2], fields[3] == "1") { Id = fields[0], Sender = sender };
        return message;
    }

    public List<Message> GetItemsByEmail(string email)
    {
        return Items.Where(item => item.Email == email).ToList();
    }

}