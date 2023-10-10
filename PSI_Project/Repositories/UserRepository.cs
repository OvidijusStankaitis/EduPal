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
            return new User (name: parts[1],surname: parts[2],email: parts[3],password: parts[4]);
        }

        public User? GetUserByEmail(string email)
        {
            return Items.FirstOrDefault(user => user.Email == email);
        }
    }
}