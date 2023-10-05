using PSI_Project.DAL;

namespace PSI_Project.Repositories
{
    public class SubjectRepository : BaseRepository<Subject>
    {
        protected override string DbFilePath => "..//PSI_Project//DB//subject.txt";

        protected override string ItemToDbString(Subject item)
        {
            return $"{item.Id};{item.Name};"; // Removed the description field
        }

        protected override Subject StringToItem(string dbString)
        {
            String[] subjectFields = dbString.Split(";");
            return new Subject(subjectFields[1]); // use only name to construct Subject
        }
        
        public Subject? GetItemByName(string subjectName)
        {
            return Items.FirstOrDefault(subject => subject.Name.Equals(subjectName, StringComparison.OrdinalIgnoreCase));
        }

        public List<Subject> GetSubjectList()
        {
            return Items.ToList();
        }

        protected override void AfterOperation()
        {
            Items.Sort((subject1, subject2) => subject1.Name.CompareTo(subject2.Name));
        }
    }
}