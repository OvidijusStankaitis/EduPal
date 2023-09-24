namespace PSI_Project;

public static class SubjectHandler
{ 
    static SubjectHandler()
    {
        SubjectList = new List<Subject>();
    }

    public static List<Subject> SubjectList { get; set; }

    public static void AddNewSubject(Subject subject)
    {
        SubjectList.Add(subject);
    }

    public static Subject CreateSubject(string subjectName, string subjectDescription)
    {
        Subject newSubject = new Subject(subjectName, subjectDescription);
        SubjectList.Add(newSubject);

        // TODO: data base + file handling
        
        return newSubject;
    }
    
    
}