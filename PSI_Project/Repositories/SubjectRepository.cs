using System.Text.Json;
using PSI_Project.Models;

namespace PSI_Project.Repositories;
public class SubjectRepository : BaseRepository<Subject>
{
    protected override string DbFilePath => "..//PSI_Project//DB//subject.txt";

    public List<Subject> GetSubjectList()
    {
        return Items.ToList();
    }

    public Subject? CreateSubject(JsonElement request)
    {
        if (request.TryGetProperty("subjectName", out var subjectNameProperty))
        {
            string? subjectName = subjectNameProperty.GetString();
            if (subjectName != null)
            {
                Subject newSubject = new Subject(subjectName);
                if(InsertItem(newSubject));
                    return newSubject;
            }
        }

        return null;
    }
    
    protected override void AfterOperation()
    {
        Items.Sort((subject1, subject2) => String.Compare(subject1.Name, subject2.Name, StringComparison.Ordinal));
    }
    
    protected override string ItemToDbString(Subject item)
    {
        return $"{item.Id};{item.Name};";
    }
    
    protected override Subject StringToItem(string dbString)
    {
        String[] subjectFields = dbString.Split(";");
        Subject newSubject = new Subject(subjectFields[1])
        {
            Id = subjectFields[0]
        };
        return newSubject;
    }
}
