using RootsApp.Models;
using RootsApp.Repositories.Implementations;
using RootsApp.Repositories.Interfaces;
using RootsApp.Services.Interfaces;

namespace RootsApp.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly IEmailService _emailService;

        public UserService(IUserRepository userRepo, IEmailService emailService)
        {
            _userRepo = userRepo;
            _emailService = emailService;
        }

        public async Task<bool> RegisterAsync(User user)
        {
            var exists = _userRepo.GetByUsername(user.Username) != null || _userRepo.GetByEmail(user.Email) != null;
            if (exists) return false;

            user.PasswordHash = HashPassword(user.PasswordHash);
            user.IsEmailConfirmed = false;
            user.CreatedAt = DateTime.UtcNow;

            _userRepo.AddUser(user);
            _userRepo.Save();

            // Generate verification code and send it
            var verificationCode = GenerateVerificationCode();
            var emailVerification = new EmailVerification
            {
                UserId = user.Id,
                VerificationCode = verificationCode,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                IsUsed = false
            };

            _userRepo.AddEmailVerification(emailVerification);
            _userRepo.Save();

            // Send the email
            await _emailService.SendVerificationEmailAsync(user.Email, user.FullName, verificationCode);

            return true;
        }

        public bool Register(User user)
        {
            var exists = _userRepo.GetByUsername(user.Username) != null || _userRepo.GetByEmail(user.Email) != null;
            if (exists) return false;

            user.PasswordHash = HashPassword(user.PasswordHash);
            _userRepo.AddUser(user);
            _userRepo.Save();
            return true;
        }

        public User Login(string username, string password)
        {
            var user = _userRepo.GetByUsername(username);
            if (user == null) return null;

            var hash = HashPassword(password);
            return user.PasswordHash == hash ? user : null;
        }

        public bool VerifyEmailCode(string email, string code)
        {
            var user = _userRepo.GetByEmail(email);
            if (user == null) return false;

            var verification = _userRepo.GetEmailVerification(user.Id, code);
            if (verification == null || verification.IsUsed == true || verification.ExpiresAt < DateTime.UtcNow)
                return false;

            user.IsEmailConfirmed = true;
            verification.IsUsed = true;
            _userRepo.Save();

            return true;
        }

        public async Task<bool> ResendVerificationCodeAsync(string email)
        {
            var user = _userRepo.GetByEmail(email);
            if (user == null || user.IsEmailConfirmed == true) return false;

            var verificationCode = GenerateVerificationCode();
            var emailVerification = new EmailVerification
            {
                UserId = user.Id,
                VerificationCode = verificationCode,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                IsUsed = false
            };

            _userRepo.AddEmailVerification(emailVerification);
            _userRepo.Save();

            await _emailService.SendVerificationEmailAsync(user.Email, user.FullName, verificationCode);
            return true;
        }

        private string HashPassword(string password)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private string GenerateVerificationCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        public void VerifyEmail(string email)
        {
            _userRepo.VerifyEmail(email);
        }
    }
}
