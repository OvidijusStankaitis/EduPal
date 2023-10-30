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
        
        public bool CheckUserLogin(JsonElement request)
        {
            if (request.TryGetProperty("email", out JsonElement emailElement) &&
                request.TryGetProperty("password", out JsonElement passwordElement))
            {
                var email = emailElement.GetString();
                var password = passwordElement.GetString();

                var user = GetUserByEmail(email);
                return (user == null || user.Password != password);
            }
            return false;
        }

        public bool CheckUserRegister(User user)
        {
            var existingUser = GetUserByEmail(user.Email);
            if (existingUser != null)
            {
                return false;
            }
            
            Add(user);
            int changes = EduPalContext.SaveChanges();

            return changes > 0;
        }
    }
}