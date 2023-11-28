using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
        
        public async Task<User?> CreateAsync(RegisterRequest registerData)
        {
            if (FindAsync(user => user.Email == registerData.Email).Result.IsNullOrEmpty())
            {
                User newUser = new User(registerData.Email, registerData.Password, registerData.Name, registerData.Surname);
                Add(newUser);
                return newUser;
            }

            return null;
        }
    }
}