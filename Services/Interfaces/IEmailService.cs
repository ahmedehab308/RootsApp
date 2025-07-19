namespace RootsApp.Services.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendVerificationEmailAsync(string email, string fullName, string verificationCode);

    }
}
