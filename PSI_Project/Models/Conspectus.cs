using PSI_Project.DAL;

namespace PSI_Project
{
    public class Conspectus : BaseEntity, IStorable
    {
        public string TopicId { get; set; } // Using properties is better than fields for data encapsulation
        public string TopicName { get; set; }
        public string Path { get; set; }
    
        public Conspectus(string topicName, string path)
        {
            TopicName = topicName;
            Path = path;
        }
    }
}
