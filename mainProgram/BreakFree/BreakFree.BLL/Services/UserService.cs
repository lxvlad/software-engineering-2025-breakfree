using System.Linq;
using BreakFree.DAL;
using BreakFree.DAL.Entities;
using BreakFree.BLL.Interfaces;

namespace BreakFree.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly BreakFreeContext _context;

        public UserService() : this(new BreakFreeContext())
        {
        }

        public UserService(BreakFreeContext context)
        {
            _context = context;
        }

        public User? Login(string email, string password)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);
        }

        public bool Register(string username, string email, string password)
        {
            if (_context.Users.Any(u => u.Email == email))
                return false;

            var user = new User { UserName = username, Email = email, Password = password };
            _context.Users.Add(user);
            _context.SaveChanges();
            return true;
        }

        public bool ChangePassword(int userId, string currentPassword, string newPassword)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null || user.Password != currentPassword) return false;

            user.Password = newPassword;
            _context.SaveChanges();
            return true;
        }

        public bool DeleteUser(int userId, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null || user.Password != password) return false;

            _context.Users.Remove(user);
            _context.SaveChanges();
            return true;
        }

        public bool UpdateUser(int userId, string newUsername, string newEmail)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null) return false;

            if (!newUsername.Equals(user.UserName, StringComparison.OrdinalIgnoreCase))
            {
                if (_context.Users.Any(u => u.UserName == newUsername && u.UserId != userId))
                    return false;
            }

            if (!newEmail.Equals(user.Email, StringComparison.OrdinalIgnoreCase))
            {
                if (_context.Users.Any(u => u.Email == newEmail && u.UserId != userId))
                    return false;
            }

            user.UserName = newUsername;
            user.Email = newEmail;
            _context.SaveChanges();

            return true;
        }

        public User? GetUserById(int id)
        {
            return _context.Users.FirstOrDefault(u => u.UserId == id);
        }
    }
}
