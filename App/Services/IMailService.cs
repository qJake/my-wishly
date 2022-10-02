using MyWishly.App.Models.Options;

namespace MyWishly.App.Services
{
    public interface IMailService
    {
        Task SendVerificationEmail(string email, string verificationLink);
    }
}