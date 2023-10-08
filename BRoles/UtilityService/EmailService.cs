using BRoles.Models;
using MailKit.Net.Smtp;
using MimeKit;
using System.Linq.Expressions;


namespace BRoles.UtilityService
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void SendEmail(EmailModel emailModel)
        {
            var emailMessage = new MimeMessage();
            var from = _configuration["EmailSettings:From"];
            emailMessage.From.Add(new MailboxAddress("foufa", from));
            emailMessage.To.Add(new MailboxAddress(emailModel.To, emailModel.To));
            emailMessage.Subject = emailModel.Subject;
            emailMessage.Body =  new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = emailModel.Content


            }; ;
            using(var client=new SmtpClient())
            {
                try{
                    client.Connect(_configuration["EmailSettings:SmtpServer"],465,true );
                    client.Authenticate(_configuration["EmailSettings:From"], _configuration["EmailSettings:Password"]);
                    client.Send(emailMessage);

                }
                catch(Exception ex)         
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

        public void SendEmailPassword(EmailModel emailModel, string password,string username)
        {
        

            var emailMessage = new MimeMessage();
            var from = _configuration["EmailSettings:From"];
            emailMessage.From.Add(new MailboxAddress("foufa", from));
            emailMessage.To.Add(new MailboxAddress(emailModel.To, emailModel.To));
            emailMessage.Subject = emailModel.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = $@"


<html>
    <head>

    </head>
    <body style=""""margin:0; padding:0;font-family: Arial, Helvetica, sans-serif;"""">
        <div style=""""height: auto;background: Linear-gradient(to top, #c9c9ff 50%, #6e6ef6 90%) no-repeat;width:400px; padding:30p"""">
        </div>

        <div>
            <h1>Reset your Password</h1>
            <hr>
            <p>hi {username},</p>
            <p>You're receiving this e-mail because an administrator has added your account.</p>
            <p>Your password is: '{password}'.</p>
 <a href=""http://localhost:4200/"" target=""_blank""
                            style="" background:#8d6efc;padding:10px;border:none;
                           color:white; border-radius: 4px;display:block;margin:0 auto; width: 50%; text-align:center; text-decoration:none"">login
                        </a>

            <p>Kind Regards,<br><br>
                bassoumi farah
            </p>
        </div>
    </body>
</html>"





            }; 
            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(_configuration["EmailSettings:SmtpServer"], 465, true);
                    client.Authenticate(_configuration["EmailSettings:From"], _configuration["EmailSettings:Password"]);
                    client.Send(emailMessage);

                }
                catch (Exception ex)
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
}

