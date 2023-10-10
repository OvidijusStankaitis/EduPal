using PSI_Project.DAL;

namespace PSI_Project.Repositories
{
    public class UserRepository : BaseRepository<User>
    {
        protected override string DbFilePath => "..//PSI_Project//DB//users.txt";

        protected override string ItemToDbString(User user)
        {
            return $"{user.Id};{user.Name};{user.Surname};{user.Email};{user.Password}";
        }

        protected override User StringToItem(string dbString)
        {
            var parts = dbString.Split(";");
            return new User
            {
                Id = parts[0],
                Name = parts[1],
                Surname = parts[2],
                Email = parts[3],
                Password = parts[4]
            };
        }

        public User? GetUserByEmail(string email)
        {
            return Items.FirstOrDefault(user => user.Email == email);
        }
    }
}