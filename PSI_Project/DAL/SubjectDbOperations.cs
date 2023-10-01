namespace PSI_Project.DAL;

public class SubjectDbOperations : EntityDbOperations<Subject>
{
    protected override string DbFilePath => "..//PSI_Project//DB//subject.txt";
    protected override string ItemToDbString(Subject item)
    {
        return $"{item.Id};{item.Name};{item.Description}";
    }

    protected override Subject StringToItem(string dbString)
    {
        String[] subjectFields = dbString.Split(new char[] { ';' });
        return new Subject(subjectFields[0], subjectFields[1], subjectFields[2]);
    }
    
    public List<Subject> GetSubjectList()
    {
        List<Subject> subjectList = new List<Subject>();
        
        using (StreamReader sr = File.OpenText(DbFilePath))
        {
            string? line;
            while ((line = sr.ReadLine()) != null)
            {
                Subject subject = StringToItem(line);
                subjectList.Add(subject);
            }
        }

        return subjectList;
    }
}