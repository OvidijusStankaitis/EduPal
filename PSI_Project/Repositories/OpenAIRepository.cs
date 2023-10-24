using PSI_Project.Data;
using PSI_Project.Models;

namespace PSI_Project.Repositories;

public class OpenAIRepository : Repository<Message>
{
    public EduPalDatabaseContext EduPalContext => Context as EduPalDatabaseContext;
    
    public OpenAIRepository(EduPalDatabaseContext context) : base(context)
    {
    }

    public List<Message> GetItemsByEmail(string email)
    {
        return EduPalContext.Messages.Where(item => item.Email == email).ToList();
    }

}