using RootsApp.Models;

namespace RootsApp.Services.Interfaces
{
    public interface IUserService
    {
        bool Register(User user);
        Task<bool> RegisterAsync(User user);

        User Login(string username, string password);
        void VerifyEmail(string email);
        bool VerifyEmailCode(string email, string code);
        Task<bool> ResendVerificationCodeAsync(string email);

    }
}
