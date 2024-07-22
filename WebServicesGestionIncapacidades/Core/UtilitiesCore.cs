using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.Text;
using System.Security.Cryptography;

namespace WebServicesGestionIncapacidades.Core
{
    public class UtilitiesCore
    {
        private readonly IConfiguration _configuration;

        public UtilitiesCore(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<bool> SendEmail(string? toAddress, string subject, string body, bool IsBodyHtml)
        {
            bool result = false;
            try
            {
                string? _smtpServer = _configuration["Email:smtpServer"];
                int _smtpPort = int.Parse(_configuration["Email:smtpPort"]);
                string? _smtpUsername = _configuration["Email:smtpUsername"];
                string? _smtpPassword = _configuration["Email:smtpPassword"];

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(_smtpUsername);
                    mail.To.Add(toAddress);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = IsBodyHtml; // Si el cuerpo del correo contiene HTML

                    using (SmtpClient smtpClient = new(_smtpServer, _smtpPort))
                    {
                        try
                        {
                            smtpClient.UseDefaultCredentials = false;
                            smtpClient.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                            smtpClient.EnableSsl = true;
                            smtpClient.Timeout = 90000; // 20 segundos

                            smtpClient.Send(mail);
                            result = true;
                        }
                        catch (Exception ex)
                        {
                            result = false;
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }
        public string GetSHA256(string value)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] hashBytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(value));
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}
