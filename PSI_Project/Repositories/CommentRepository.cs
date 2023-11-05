using System.Text.Json;
using PSI_Project.Data;
using PSI_Project.Exceptions;
using PSI_Project.Models;
namespace PSI_Project.Repositories;
public class CommentRepository : Repository<Comment>
{
    public EduPalDatabaseContext EduPalContext => Context as EduPalDatabaseContext;

    public CommentRepository(EduPalDatabaseContext context) : base(context)
    {
    }
    
    public List<Comment> GetAllCommentsOfTopic(string topicId)
    {
        return EduPalContext.Comments
            .Select(comment => comment)
            .Where(comment => comment.TopicId.Equals(topicId)).ToList();
    }
    
    public Comment? GetItemById(string itemId)  
    {
        return EduPalContext.Comments
            .FirstOrDefault(comment => comment.Id.Equals(itemId));
    }
    
    public bool Remove(string commentId)    // TODO: resolve, might be not not needed
    {
        try
        {
            Comment? comment = Get(commentId);
            if (comment is null)
                return false;

            Remove(comment);
            int changes = EduPalContext.SaveChanges();

            return changes > 0;
        }
        catch (Exception ex)
        {
            // TODO: log errors
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            throw new EntityDeletionException("Error occured while trying to delete comment", ex);
        }
    }
    
}