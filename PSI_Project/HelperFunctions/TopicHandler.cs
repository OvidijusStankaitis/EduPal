using PSI_Project.DAL;

namespace PSI_Project.HelperFunctions;

public class TopicHandler : BaseHandler<Topic, TopicDbOperations>
{
    public override TopicDbOperations DbOperations { get; set; } = new TopicDbOperations();

    public TopicHandler()
    {
        Items = DbOperations.ReadAllItemsFromDB();
    }

    public List<Topic> GetTopicListBySubjectId(string subjectId)
    {
        return DbOperations.GetTopicListBySubjectId(subjectId);
    }
    
}