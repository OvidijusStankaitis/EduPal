using Microsoft.EntityFrameworkCore;
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
        
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await EduPalContext.Users.FirstOrDefaultAsync(user => user.Email.Equals(email));
        }
        
        public async Task<string?> CheckUserLoginAsync(string email, string password)
        {
            User? user = await GetUserByEmailAsync(email);
            if (user != null && user.Password == password)
            {
                return user.Id;
            }
            return null;
        }

        public async Task<string?> CheckUserRegisterAsync(UserCreationDTO? newUserData)
        {
            if (newUserData is null)
            {
                return null;
            }

            string? email = newUserData.Email;
            if (email == null || await GetUserByEmailAsync(email) != null)
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
            
            int changes = Add(newUser);
            //int changes = EduPalContext.SaveChanges();

            if (changes > 0)
            {
                return newUser.Id;
            }

            return null;
        }
    }
}