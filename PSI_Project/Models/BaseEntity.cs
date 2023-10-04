namespace PSI_Project
{
    public class BaseEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public BaseEntity()
        {
            Id = Guid.NewGuid().ToString(); // Generate a unique Id
        }

        public BaseEntity(string name)
        {
            Id = Guid.NewGuid().ToString(); // Generate a unique Id
            Name = name;
        }
    }
}
