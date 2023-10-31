using System.Text.Json;
using PSI_Project.Data;
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
            return EduPalContext.Users.FirstOrDefault(user => user.Email == email);
        }
        
        public string? CheckUserLogin(JsonElement request)
        {
            if (request.TryGetProperty("email", out JsonElement emailElement) &&
                request.TryGetProperty("password", out JsonElement passwordElement))
            {
                string? email = emailElement.GetString();
                string? password = passwordElement.GetString();
                
                if (email == null || password == null)
                {
                    return null;
                }
                
                User? user = GetUserByEmail(email);
                if (user != null && user.Password == password)
                {
                    return user.Id;
                }
            }
            
            return null;
        }

        public string? CheckUserRegister(JsonElement request)
        {
            if (request.TryGetProperty("name", out JsonElement nameElement) &&
                request.TryGetProperty("surname", out JsonElement surnameElement) &&
                request.TryGetProperty("email", out JsonElement emailElement) &&
                request.TryGetProperty("password", out JsonElement passwordElement))
            {
                string? email = emailElement.GetString();
                if (email == null || GetUserByEmail(email) != null)
                {
                    return null;
                }

                string? name = nameElement.GetString();
                string? surname = surnameElement.GetString();
                string? password = passwordElement.GetString();
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
            }

            return null;
        }
    }
}