using System.Text.Json;
using PSI_Project.Data;
using PSI_Project.DTO;
using PSI_Project.Models;

namespace PSI_Project.Repositories
{
    public class UserRepository : Repository<User>
    {
        public EduPalDatabaseContext EduPalContext => Context as EduPalDatabaseContext;
        public UserRepository(EduPalDatabaseContext context) : base(context)
        {
        }
        
        public User? GetUserByEmail(string email)
        {
            return EduPalContext.Users.FirstOrDefault(user => user.Email.Equals(email));
        }
        
        public string? CheckUserLogin(string email, string password)
        {
            User? user = GetUserByEmail(email);
            if (user != null && user.Password == password)
            {
                return user.Id;
            }
    
            return null;
        }

        public string? CheckUserRegister(UserCreationDTO? newUserData)
        {
            if (newUserData is null)
            {
                return null;
            }
            
            string? email = newUserData.Email;
            if (email == null || GetUserByEmail(email) != null)
            {
                return null;
            }

            string? name = newUserData.Name;
            string? surname = newUserData.Surname;
            string? password = newUserData.Password;
            if (name == null || surname == null || password == null)
            {
                return null;
            }
            
            User newUser = new User(email, password, name, surname);
            
            Add(newUser);
            int changes = EduPalContext.SaveChanges();
            if (changes > 0)
            {
                return newUser.Id;
            }

            return null;
        }
    }
}