namespace PSI_Project;

public static class SubjectHandler
{ 
    static SubjectHandler()
    {
        SubjectList = new List<Subject>();
        ReadAllSubjectsFromDB(); // when launching the program all the subjects are saved into the SubjectList from the DB
    }

    public static List<Subject> SubjectList { get; set; }

    // public static void AddNewSubject(Subject subject) // do we need to specify a new method for just one Add method?
    // {
    //     SubjectList.Add(subject);
    // }

    public static Subject CreateSubject(string subjectName, string subjectDescription)
    {
        Subject newSubject = new Subject(subjectName, subjectDescription);
        
        // TODO: data base + file handling
        
        SubjectList.Add(newSubject);
        WriteSubjectIntoDB(newSubject);
        
        return newSubject;
    }

    private static void WriteSubjectIntoDB(Subject subject) //void? what happens in case of an error? 
    {
        using (var streamWriter = new StreamWriter("..//DB//SubjectInfromation.txt",true)) //true makes it possible to append(not overwrite text) to a file 
        {
            streamWriter.WriteLine($"{subject.Name} {subject.Description}");//is async method needed?
        }
    }

    public static void ReadAllSubjectsFromDB()
    {
        using (var streamReader = new StreamReader("..//DB//SubjectInfromation.txt"))
        {
            streamReader.ReadLine(); //skipping the first line with the info about subjects (fields/properties)
            string? subjectInfo = streamReader.ReadLine();
            while (subjectInfo != null)
            {
                String[] subjectFields = subjectInfo.Split(new char[] { ' ' });
                SubjectList.Add(new Subject(subjectFields[0],subjectFields[1]));
            }
        }
    }
    
    
}