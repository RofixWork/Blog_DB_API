using MailKit.Net.Smtp;
using MimeKit;

namespace Blog_DB_API.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Send(Message message)
        {
            var email = CreateEmailMessage(message);
            SendEmail(email);
        }

        public MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessge = new MimeMessage(); 
            emailMessge.To.AddRange(message.To);
            emailMessge.From.Add(new MailboxAddress("email", _configuration.GetSection("EmailConfig:From").Value));
            emailMessge.Subject = message.Subject;
            emailMessge.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content };

            return emailMessge;
        }

        public void SendEmail(MimeMessage mailMessage)
        {
            using var client = new SmtpClient();
            try
            {
                client.Connect(_configuration.GetSection("EmailConfig:Smtp").Value, Convert.ToInt32(_configuration.GetSection("EmailConfig:Port").Value), true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(_configuration.GetSection("EmailConfig:Username").Value, _configuration.GetSection("EmailConfig:Password").Value);
                client.Send(mailMessage);
            }
            catch
            {
                throw;
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }
        }
    }
}
