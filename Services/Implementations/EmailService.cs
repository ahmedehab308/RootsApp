using System;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using RootsApp.Services.Interfaces;

namespace RootsApp.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendVerificationEmailAsync(string email, string fullName, string verificationCode)
        {
            try
            {
                var smtpSettings = _configuration.GetSection("EmailSettings");

                using (var client = new SmtpClient())
                {
                    client.Host = smtpSettings["SmtpHost"];
                    client.Port = int.Parse(smtpSettings["SmtpPort"]);
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(
                        smtpSettings["Username"],
                        smtpSettings["Password"]
                    );

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(smtpSettings["FromAddress"], smtpSettings["FromName"]),
                        Subject = "Email Verification - RootsApp",
                        Body = CreateEmailBody(fullName, verificationCode),
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(email);
                    await client.SendMailAsync(mailMessage);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private string CreateEmailBody(string fullName, string verificationCode)
        {
            return $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <h2 style='color: #333;'>Hello {fullName},</h2>
                    <p>Thank you for registering at RootsApp!</p>
                    <p>To verify your email address, please use the following code:</p>
                    
                    <div style='background-color: #f4f4f4; padding: 20px; text-align: center; margin: 20px 0;'>
                        <h1 style='color: #007bff; font-size: 36px; margin: 0; letter-spacing: 5px;'>{verificationCode}</h1>
                    </div>
                    
                    <p>This code is valid for 15 minutes only.</p>
                    <p>If you did not create this account, please ignore this email.</p>
                    
                    <hr style='margin: 30px 0;'>
                    <p style='color: #666; font-size: 12px;'>Support Team - RootsApp</p>
                </div>";
        }
    }
}
