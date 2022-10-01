using MyWishly.App.Models;

namespace MyWishly.App.Services
{
    public interface IAuthService
    {
        Task<User> GetUser(string email);
        Task RegisterUser(User newUser);
    }
}