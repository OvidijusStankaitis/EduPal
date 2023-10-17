using System.Text.Json;

namespace PSI_Project.Repositories
{
    public class UserRepository : BaseRepository<User>
    {
        protected override string DbFilePath => "..//PSI_Project//DB//users.txt";

        protected override string ItemToDbString(User user)
        {
            return $"{user.Id};{user.Name};{user.Surname};{user.Email};{user.Password};";
        }

        protected override User StringToItem(string dbString)
        {
            var parts = dbString.Split(";");
            return new User (name: parts[1],surname: parts[2],email: parts[3],password: parts[4]);
        }

        public User? GetUserByEmail(string email)
        {
            return Items.FirstOrDefault(user => user.Email == email);
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
            return InsertItem(user);
        }
    }
}