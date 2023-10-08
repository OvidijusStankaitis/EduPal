using PSI_Project.DAL;

namespace PSI_Project
{
    public class Conspectus : BaseEntity, IStorable
    {
        public string TopicName { get; set; }
        public string Path { get; set; }
        public int Rating { get; set; }
    
        public Conspectus(string topicName, string path, int rating = 0)
        {
            TopicName = topicName;
            Path = path;
            Rating = rating;
        }
    }
}
