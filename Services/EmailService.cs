using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using TKGroopBG.Models;

namespace TKGroopBG.Services
{
    public interface IEmailService
    {
        Task SendOrderEmailAsync(OrderRequest order);
    }

    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _settings;

        public EmailService(IOptions<SmtpSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendOrderEmailAsync(OrderRequest order)
        {
            var body = BuildBody(order);

            using var client = new SmtpClient(_settings.Host, _settings.Port)
            {
                EnableSsl = _settings.EnableSsl,
                Credentials = new NetworkCredential(_settings.Username, _settings.Password)
            };

            var mail = new MailMessage
            {
                From = new MailAddress(_settings.From),
                Subject = "Нова заявка от сайта TKGroopBG",
                Body = body,
                IsBodyHtml = false
            };

            mail.To.Add(_settings.To);

            await client.SendMailAsync(mail);
        }

        private string BuildBody(OrderRequest o)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Нова заявка от сайта:");
            sb.AppendLine($"Име: {o.CustomerName}");
            sb.AppendLine($"Телефон: {o.Phone}");
            sb.AppendLine($"Имейл: {o.Email}");
            sb.AppendLine($"Адрес: {o.Address}");
            sb.AppendLine($"Коментар: {o.Comment}");
            sb.AppendLine();
            sb.AppendLine("Съдържание на количката (JSON):");
            sb.AppendLine(o.CartJson ?? "(празно)");
            return sb.ToString();
        }
    }
}

