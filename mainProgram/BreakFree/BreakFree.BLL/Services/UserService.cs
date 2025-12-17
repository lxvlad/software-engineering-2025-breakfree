namespace BreakFree.BLL.Services
{
    using System.Linq;
    using BreakFree.BLL.Interfaces;
    using BreakFree.DAL;
    using BreakFree.DAL.Entities;

    public class UserService : IUserService
    {
        private readonly BreakFreeContext context;

        public UserService(BreakFreeContext? injectedContext = null)
        {
            this.context = injectedContext ?? new BreakFreeContext();
        }

        public User? Login(string email, string password)
        {
            return this.context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);
        }

        public bool Register(string username, string email, string password)
        {
            if (this.context.Users.Any(u => u.Email == email))
            {
                return false;
            }

            var user = new User
            {
                UserName = username,
                Email = email,
                Password = password,
            };

            this.context.Users.Add(user);
            this.context.SaveChanges();

            return true;
        }

        public bool ChangePassword(int userId, string currentPassword, string newPassword)
        {
            var user = this.context.Users.FirstOrDefault(u => u.UserId == userId);

            if (user == null || user.Password != currentPassword)
            {
                return false;
            }

            user.Password = newPassword;
            this.context.SaveChanges();

            return true;
        }

        public bool DeleteUser(int userId, string password)
        {
            var user = this.context.Users.FirstOrDefault(u => u.UserId == userId);

            if (user == null || user.Password != password)
            {
                return false;
            }

            this.context.Users.Remove(user);
            this.context.SaveChanges();

            return true;
        }

        public bool UpdateUser(int userId, string newUsername, string newEmail)
        {
            var user = this.context.Users.FirstOrDefault(u => u.UserId == userId);

            if (user == null)
            {
                return false;
            }

            if (!newUsername.Equals(user.UserName, System.StringComparison.OrdinalIgnoreCase) &&
                this.context.Users.Any(u => u.UserName == newUsername && u.UserId != userId))
            {
                return false;
            }

            if (!newEmail.Equals(user.Email, System.StringComparison.OrdinalIgnoreCase) &&
                this.context.Users.Any(u => u.Email == newEmail && u.UserId != userId))
            {
                return false;
            }

            user.UserName = newUsername;
            user.Email = newEmail;

            this.context.SaveChanges();
            return true;
        }

        public User? GetUserById(int id)
        {
            return this.context.Users.FirstOrDefault(u => u.UserId == id);
        }
    }
}
