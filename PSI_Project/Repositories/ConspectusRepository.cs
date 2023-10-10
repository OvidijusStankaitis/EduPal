namespace PSI_Project.Repositories
{
    public class ConspectusRepository : BaseRepository<Conspectus>
    {
        protected override string DbFilePath => "..//PSI_Project//DB//conspectus.txt";

        protected override string ItemToDbString(Conspectus item)
        {
            return $"{item.Id};{item.TopicName};{item.Path};";
        }

        protected override Conspectus StringToItem(string dbString)
        {
            String[] fields = dbString.Split(";");
            return new Conspectus(fields[0], fields[1]);
        }

        public List<Conspectus> GetConspectusListByTopicName(string topicName)
        {
            return Items.Where(conspectus => conspectus.TopicName == topicName).ToList();
        }

        public bool IsFileUsedInOtherTopics(string filePath)
        {
            return Items.Any(conspectus => conspectus.Path == filePath);
        }
    }
}