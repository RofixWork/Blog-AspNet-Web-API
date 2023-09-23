using MimeKit;

namespace demerge_blog_API.Services
{
    public class Message
    {
        public Message(string to, string? subject, string? content)
        {
            To = new MailboxAddress("email", to);
            Subject = subject;
            Content = content;
        }

        public MailboxAddress? To { get; set; }
        public string? Subject { get; set; }
        public string? Content { get; set; }
    }
}
