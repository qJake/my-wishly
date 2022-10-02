using Microsoft.Extensions.Options;
using MyWishly.App.Models.Options;
using System.Net;
using System.Net.Mail;

namespace MyWishly.App.Services
{
    public class MailService : IMailService
    {
        public SmtpSettings SmtpOptions { get; }

        public MailService(IOptions<SmtpSettings> options)
        {
            SmtpOptions = options.Value;
        }

        private bool AreOptionsValid()
        {
            return !string.IsNullOrWhiteSpace(SmtpOptions.FromAddress)
                && !string.IsNullOrWhiteSpace(SmtpOptions.SmtpHost)
                && !string.IsNullOrWhiteSpace(SmtpOptions.ApiKey)
                && !string.IsNullOrWhiteSpace(SmtpOptions.Username)
                && SmtpOptions.SmtpPort > 0;
        }


        public async Task SendVerificationEmail(string email, string verificationLink)
        {
            if (!AreOptionsValid())
            {
                throw new InvalidOperationException("One or more SMTP options are missing. Check app configuration.");
            }

            var body = $@"
<!DOCTYPE html>
<html>
    <head>
        <style type=""text/css"">
            html, body
            {{
                font-family: Verdana, sans-serif;
                font-size: 14px;
            }}
        </style>
    </head>
    <body>
        <h3>My Wishly</h3>
        <p><b>Thank you for signing up for My Wishly!</b></p>
        <p>To activate your account, please click the following link:</p>
        <p><a href=""{verificationLink}"">{verificationLink}</a></p>
        <p><em>If you didn't sign up for My Wishly or you aren't sure what this is, you may safely disregard this e-mail.</em></p>
        <p>-- The My Wishly Team</p>
    </body>
</html>";

            using (var smtpClient = new SmtpClient(SmtpOptions.SmtpHost, SmtpOptions.SmtpPort)
            {
                EnableSsl = SmtpOptions.UseTls,
                Credentials = new NetworkCredential(SmtpOptions.Username, SmtpOptions.ApiKey),
                Timeout = 10000
            })
            {
                var mm = new MailMessage(new MailAddress(SmtpOptions.FromAddress!, "My Wishly"), new MailAddress(email))
                {
                    Subject = "Please Verify Your Email Address on My Wishly",
                    Body = body,
                    IsBodyHtml = true
                };

                await smtpClient.SendMailAsync(mm);
            }
        }
    }
}
