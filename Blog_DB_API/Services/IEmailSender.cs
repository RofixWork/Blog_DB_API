using MimeKit;

namespace Blog_DB_API.Services
{
    public interface IEmailSender
    {
        void Send(Message message);
        MimeMessage CreateEmailMessage(Message message);
        void SendEmail(MimeMessage mailMessage);
    }
}
