using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceWebApp.Services
{
    // This class is used by the application to send Email and SMS
    // when you turn on two-factor authentication in ASP.NET Identity.
    // For more details see this link https://go.microsoft.com/fwlink/?LinkID=532713
    public class AuthMessageSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var @from = new MailAddress("no-reply@CustomClientCredentialHost.info", "My Awesome SMS Admin");
            var to = new MailAddress(email);

            var mailMessage = new MailMessage(@from, to)
            {
                Subject = subject,
                Body = message,
                BodyEncoding = Encoding.UTF8,
                IsBodyHtml = false,

            };

            mailMessage.Bcc.Add("boss@company.com");
            SmtpClient client = new SmtpClient("127.0.0.1", 32525);
            NetworkCredential info = new NetworkCredential("mail@jonathanchannon.com", "reallysecurepassword");
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = info;

            // Plug in your SMS service here to send a text message.

            client.Send(mailMessage);

        }

        public async Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            await SendEmailAsync($"{number}@somedomain.com", "Validate Your Identity", message);
        }
    }
}
