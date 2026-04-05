using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using TKGroopBG.Models;

namespace TKGroopBG.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _settings;

        public EmailService(IOptions<SmtpSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendOrderEmailAsync(OrderRequest order)
        {
            var sb = new StringBuilder();
            sb.Append("<h2>Нова поръчка от TKGroopBG</h2>");
            sb.Append($"<p><b>Клиент:</b> {order.FirstName} {order.LastName}</p>");
            sb.Append($"<p><b>Телефон:</b> {order.Phone}</p>");
            sb.Append($"<p><b>Град:</b> {order.City}</p>");
            sb.Append("<table border='1' style='border-collapse:collapse; width:100%;'>");
            sb.Append("<tr style='background:#eee;'><th>Продукт</th><th>Детайли</th><th>Цена</th></tr>");

            foreach (var item in order.Items)
            {
                sb.Append("<tr>");
                sb.Append($"<td>{item.ProductName}</td>");
                sb.Append($"<td>{item.Details}</td>");
                sb.Append($"<td>{item.Price} лв.</td>");
                sb.Append("</tr>");
            }
            sb.Append("</table>");
            sb.Append($"<h3>Обща сума: {order.Items.Sum(x => x.Price)} лв.</h3>");

            using var client = new SmtpClient(_settings.Host, _settings.Port)
            {
                EnableSsl = _settings.EnableSsl,
                Credentials = new NetworkCredential(_settings.Username, _settings.Password)
            };

            var mail = new MailMessage(_settings.From, _settings.To, "Нова поръчка от сайта", sb.ToString())
            {
                IsBodyHtml = true
            };

            await client.SendMailAsync(mail);
        }
    }
}