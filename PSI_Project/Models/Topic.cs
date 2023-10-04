using PSI_Project.DAL;

namespace PSI_Project
{
    public class Topic : BaseEntity, IStorable
    {
        public string SubjectId { get; set; } // You may want to use properties instead of fields
        public string SubjectName { get; set; }

        public Topic(string name, string subjectName) : base(name)
        {
            SubjectName = subjectName;
        }
    }
}