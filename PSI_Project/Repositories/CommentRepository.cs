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
        return Find(comment => comment.TopicId == topicId).ToList();
    }
    
    public Comment? GetItemById(string itemId)  
    {
        return Find(comment => comment.Id.Equals(itemId)).FirstOrDefault();
    }
    
    public bool Remove(string commentId) 
    {
        Comment comment = Get(commentId);
        int changes = Remove(comment);
        //int changes = EduPalContext.SaveChanges();
        return changes > 0;
    }
}