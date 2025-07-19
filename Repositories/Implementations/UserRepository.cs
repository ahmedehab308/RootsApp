using RootsApp.Models;
using RootsApp.Repositories.Interfaces;

namespace RootsApp.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly RootsDbContext _context;

        public UserRepository(RootsDbContext context)
        {
            _context = context;
        }

        public User GetByUsername(string username)
        {
            return _context.Users.FirstOrDefault(u => u.Username == username);
        }

        public User GetByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }

        public void AddUser(User user)
        {
            _context.Users.Add(user);
        }

        public void AddEmailVerification(EmailVerification emailVerification)
        {
            _context.EmailVerifications.Add(emailVerification);
        }

        public EmailVerification GetEmailVerification(int userId, string code)
        {
            return _context.EmailVerifications
                .FirstOrDefault(ev => ev.UserId == userId &&
                               ev.VerificationCode == code &&
                               ev.IsUsed == false);
        }
        public void VerifyEmail(string email)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user != null)
            {
                user.IsEmailConfirmed = true;
                _context.SaveChanges();
            }
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
