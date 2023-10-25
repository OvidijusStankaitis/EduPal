using System.ComponentModel.DataAnnotations;
using PSI_Project.Models;

namespace PSI_Project
{
    public class User : BaseEntity
    {
        private string _name;
        private string _surname;
        public string Email { get; set; }
        public string Password { get; set; }

        public string Name
        {
            get => _name;
            set
            {
                if (!value.IsValidPersonName())
                    throw new ValidationException($"Invalid data: \"{value}\". Name should consist only of alphabetic characters");
                
                _name = value;
            }
        }

        public string Surname
        {
            get => _surname;
            set
            {
                if (!value.IsValidPersonName())
                    throw new ValidationException($"Invalid data: \"{value}\". Surname should consist only of alphabetic characters");
                
                _surname = value;
            }
        }

        public User(string email, string password, string name, string surname)
        {
            Email = email;
            Password = password;
            Name = name;
            Surname = surname;
        }
    }
}