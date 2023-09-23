using MimeKit;

namespace demerge_blog_API.Services
{
    public interface IEmailSender
    {
        void SendEmail(Message message);
        MimeMessage CreateEmailMessage(Message message);
        void Send(MimeMessage message);    

    }
}
