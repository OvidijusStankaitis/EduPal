namespace PSI_Project.Repositories
{
    public class TopicRepository : BaseRepository<Topic>
    {
        protected override string DbFilePath => "..//PSI_Project//DB//topic.txt";

        protected override string ItemToDbString(Topic item)
        {
            return $"{item.Id};{item.SubjectName};{item.Name};";
        }

        protected override Topic StringToItem(string dbString)
        {
            String[] topicFields = dbString.Split(";");

            return new Topic(topicFields[2], topicFields[1]);
        }

        public List<Topic> GetTopicsBySubjectName(string subjectName)
        {
            return Items.Where(topic => topic.SubjectName.Equals(subjectName)).ToList();
        }

        public Topic? GetItemByName(string topicName)
        {
            return Items.FirstOrDefault(topic => topic.Name.Equals(topicName));
        }
    }
}