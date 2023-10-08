namespace PSI_Project
{
    public class BaseEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public BaseEntity()
        {
            Id = Guid.NewGuid().ToString();
        }

        public BaseEntity(string name)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
        }
    }
}
