using BRoles.Models;

namespace BRoles.UtilityService
{
    public interface IEmailService
    {
        void SendEmail(EmailModel emailModel);
        void SendEmailPassword(EmailModel emailModel,string a,string b);
    }
}
