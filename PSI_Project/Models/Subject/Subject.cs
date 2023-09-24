namespace PSI_Project;

public class Subject
{
    public Subject(string subjectName, string subjectDescription)
    {
        Name = subjectName;
        Description = subjectDescription;
    }
    public string Name {get; set;}
    public string Description {get; set;}
}
