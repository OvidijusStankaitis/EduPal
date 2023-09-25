namespace PSI_Project;

public static class TopicHandler
{
    static TopicHandler()
    {
        TopicList = new List<Topic>();
        ReadAllTopicsFromDB();
    }
    public static List<Topic> TopicList { get; set; }

    // public static void AddNewTopic(Topic topic)
    // {
    //     TopicList.Add(topic);
    // }
    
    public static Topic CreateTopic(string topicName, string topicDescription, string topicSubjectName) //subject = the subject in which topic is created
    {
        Topic newTopic = new Topic(topicName, topicDescription, topicSubjectName);

        // TODO: data base + file handling
        //creating a method for READING FROM db (.txt file from now) all the info about the Topics stored 
        //creating a method for WRITING INTO db (.txt file from now) all the info about the Topics stored
        TopicList.Add(newTopic);

        
        return newTopic;
    }
    
    private static void WriteTopicIntoDB(Topic topic) //void? what happens in case of an error? 
    {
        using (var streamWriter = new StreamWriter("..//DB//TopicInfromation.txt",true)) //true makes it possible to append(not overwrite text) to a file 
        {
            streamWriter.WriteLine($"{topic.Name} {topic.Description} {topic.SubjectName}");//is async method needed?
        }
    }

    public static void ReadAllTopicsFromDB()
    {
        using (var streamReader = new StreamReader("..//DB//TopicInfromation.txt"))
        {
            streamReader.ReadLine();
            string? topicInfo = streamReader.ReadLine();
            while (topicInfo != null)
            {
                String[] topicFields = topicInfo.Split(new char[] { ' ' });
                TopicList.Add(new Topic(topicFields[0],topicFields[1], topicFields[2]));
            }
        }
    }
}