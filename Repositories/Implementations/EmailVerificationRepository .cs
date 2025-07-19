using Microsoft.EntityFrameworkCore;
using RootsApp.Models;
using RootsApp.Repositories.Interfaces;

namespace RootsApp.Repositories.Implementations
{
    public class EmailVerificationRepository : IEmailVerificationRepository
    {
        private readonly RootsDbContext _context;

        public EmailVerificationRepository(RootsDbContext context)
        {
            _context = context;
        }

        public void AddVerification(EmailVerification verification)
        {
            _context.EmailVerifications.Add(verification);
        }

        public EmailVerification GetByCode(string code)
        {
            return _context.EmailVerifications
                .Include(ev => ev.User)
                .FirstOrDefault(ev => ev.VerificationCode == code && ev.IsUsed == false);
        }

        public void MarkAsUsed(int id)
        {
            var verification = _context.EmailVerifications.Find(id);
            if (verification != null)
            {
                verification.IsUsed = true;
            }
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}