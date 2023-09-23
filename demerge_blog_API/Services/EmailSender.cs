using MailKit.Net.Smtp;
using MimeKit;

namespace demerge_blog_API.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendEmail(Message message)
        {
            var email = CreateEmailMessage(message);
            Send(email);
        }

        public MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.To.Add(message.To);
            emailMessage.From.Add(new MailboxAddress("email", _configuration.GetSection("EmailConfig:From").Value!));
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content };
            return emailMessage;
        }

        public void Send(MimeMessage email)
        {
            using var smtp = new SmtpClient();
            smtp.Connect(_configuration.GetSection("EmailConfig:Smtp").Value!, Convert.ToInt32(_configuration.GetSection("EmailConfig:Port").Value!), true);
            smtp.Authenticate(_configuration.GetSection("EmailConfig:Username").Value!, _configuration.GetSection("EmailConfig:Password").Value!);
            smtp.Send(email);
            smtp.Disconnect(true);
            
        }
    }
}
