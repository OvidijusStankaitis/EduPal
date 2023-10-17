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
                if (!_name.IsValidPersonName())
                    throw new Exception("invalid first name");

                _name = value;
            }
        }

        public string Surname
        {
            get => _surname;
            set
            {
                if (_surname.IsValidPersonName())
                    throw new Exception("invalid last name");
            
                _surname = value;
            }
        }

        public User(string email, string password, string name, string surname): base()
        {
            Email = email;
            Password = password;
            Name = name;
            Surname = surname;
        }
    }
}