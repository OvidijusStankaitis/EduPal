using PSI_Project.Models;

namespace PSI_Project
{
    public class User : BaseEntity
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Surname { get; set; }

        public User(string email, string password, string name, string surname): base(name)
        {
            Email = email;
            Password = password;
            Name = name;
            Surname = surname;
        }
    }
}