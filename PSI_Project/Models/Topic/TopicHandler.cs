namespace PSI_Project;

public static class TopicHandler
{
    static TopicHandler()
    {
        TopicList = new List<Topic>();
    }
    public static List<Topic> TopicList { get; set; }

    public static void AddNewTopic(Topic topic)
    {
        TopicList.Add(topic);
    }
    
    public static Topic CreateTopic(string topicName, string topicDescription, Subject topicSubject) //subject = the subject in which topic is created
    {
        Topic newTopic = new Topic(topicName, topicDescription, topicSubject);
        TopicList.Add(newTopic);

        // TODO: data base + file handling
        
        return newTopic;
    }
}