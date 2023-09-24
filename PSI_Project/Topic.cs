namespace PSI_Project;

public class Topic
{
    public Topic(string topicName, string topicDescription)
    {
        Name = topicName;
        Description = topicDescription;
        Rating = 0;
        Comments = new List<string>();
    }
    public short Rating { get; set; } 
    public string Name {get; set;}
    public string Description {get; set;}
    
    public List<String> Comments { get; set;}
    
    public void DownloadFileFromURL(string URL) //SHOULD BE USED IN Task class instead?
    {
        using (var client = new HttpClient())
        {
            using (var stream = client.GetStreamAsync("http://www.mif.vu.lt/katedros/se/Sandai/13_14/4s_PSI_II_LT.pdf"))//URL must be inserted here
            {
                using (var fileStream = new FileStream("C:\\Users\\denol\\OneDrive\\Desktop\\localfile.pdf", FileMode.OpenOrCreate))
                {
                    stream.Result.CopyTo(fileStream);
                }
            }
        }
    }
}