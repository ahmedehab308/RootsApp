using RootsApp.Models;

namespace RootsApp.Repositories.Interfaces
{
    public interface IUserRepository
    {
        User GetByUsername(string username);
        User GetByEmail(string email);
        void AddUser(User user);
        void AddEmailVerification(EmailVerification emailVerification);
        EmailVerification GetEmailVerification(int userId, string code);
        void VerifyEmail(string email);

        void Save();
    }
}
