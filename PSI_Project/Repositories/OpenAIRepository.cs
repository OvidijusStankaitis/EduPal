using System.IO;
using PSI_Project.Models;

namespace PSI_Project.Repositories;

public class OpenAIRepository : BaseRepository<Message>
{
    protected override string DbFilePath => "..//PSI_Project//DB//openai.txt";

    protected override string ItemToDbString(Message item)
    {
        return $"{item.Id};{item.Text};{item.IsUserMessage}";
    }
    
    protected override Message StringToItem(string dbString)
    {
        var fields = dbString.Split(';');
        return new Message(fields[1], bool.Parse(fields[2])) { Id = fields[0] };
    }
    
    public List<Message> GetAllItems()
    {
        return Items;
    }
}