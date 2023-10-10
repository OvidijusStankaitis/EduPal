namespace PSI_Project
{
    public class Topic : BaseEntity
    {
        public string SubjectId { get; set; }
        public string SubjectName { get; set; }

        public Topic(string name, string subjectName) : base(name)
        {
            SubjectName = subjectName;
        }
    }
}