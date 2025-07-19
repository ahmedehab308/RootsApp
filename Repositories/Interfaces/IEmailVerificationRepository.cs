using RootsApp.Models;

namespace RootsApp.Repositories.Interfaces
{
    public interface IEmailVerificationRepository
    {
        void AddVerification(EmailVerification verification);
        EmailVerification GetByCode(string code);
        void MarkAsUsed(int id);
        void Save();
    }
}
