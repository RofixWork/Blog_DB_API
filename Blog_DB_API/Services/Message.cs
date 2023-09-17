using MimeKit;
using System.Net.Mail;

namespace Blog_DB_API.Services
{
    public class Message
    {
        public Message(IEnumerable<string>? to, string? subject, string? content)
        {
            To = new();
            if(to is not null)
            {
                To.AddRange(to.Select(email => new MailboxAddress("email", email)));
            }
            Subject = subject;
            Content = content;
        }

        public List<MailboxAddress>? To { get; set; }
        public string? Subject { get; set; }
        public string? Content { get; set; }
    }
}
