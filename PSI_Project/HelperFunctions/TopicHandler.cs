using PSI_Project.DAL;

namespace PSI_Project.HelperFunctions;

public class TopicHandler : BaseHandler<Topic>
{
    public override EntityDbOperations<Topic> EntityDbOperations { get; set; } = new TopicDbOperations();
    public TopicHandler()
    {
        // ReadAllItemsFromDB();
    }
}