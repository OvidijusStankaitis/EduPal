using PSI_Project.DAL;
using PSI_Project.HelperFunctions;

namespace PSI_Project;

public class ConspectusHandler : BaseHandler<Conspectus, ConspectusDbOperations>
{
    public override ConspectusDbOperations DbOperations { get; set; } = new ConspectusDbOperations();
    
    public List<Conspectus> GetConspectusListByTopicName(string topicName)
    {
        return DbOperations.GetConspectusListByTopicName(topicName);
    }

}