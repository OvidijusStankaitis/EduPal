using PSI_Project.DAL;

namespace PSI_Project.Repositories
{
    public class ConspectusRepository : BaseRepository<Conspectus>
    {
        protected override string DbFilePath => "..//PSI_Project//DB//conspectus.txt";

        protected override string ItemToDbString(Conspectus item)
        {
            return $"{item.Id};{item.TopicName};{item.Path};{item.Rating};";
        }

        protected override Conspectus StringToItem(string dbString)
        {
            String[] fields = dbString.Split(";");
            return new Conspectus(topicName:fields[1], path:fields[2], rating:int.Parse(fields[3]));
        }

        public List<Conspectus> GetConspectusListByTopicName(string topicName)
        {
            return Items.Where(conspectus => conspectus.TopicName == topicName).ToList();
        }

        public bool IsFileUsedInOtherTopics(string filePath)
        {
            return Items.Any(conspectus => conspectus.Path == filePath);
        }

        // Changes rating of a conspectus in accordance with its Id and updates the Topic DB
        //(when should it update the whole topic DB to keep its records up to date? - everytime when rating of a topic is changed?)
        public bool ChangeRating(string conspectusId, bool toIncrease)
        {
            Conspectus? conspectus = GetItemById(conspectusId);
            if (conspectus != null)
            {
                int conspectusIndex = Items.IndexOf(conspectus);
                Items[conspectusIndex].Rating =
                    toIncrease ? Items[conspectusIndex].Rating++ : Items[conspectusIndex].Rating--;
                UpdateDB(); // should it be here?
                return true;
            }
            return false;
        }
    }
}