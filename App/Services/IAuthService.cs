using MyWishly.App.Models;

namespace MyWishly.App.Services
{
    public interface IAuthService
    {
        Task<User> GetUser(string email);
        Task<User> GetUser(Guid userId);
        Task<User> RegisterUser(User newUser);
        Task<User> UpdateUser(User user);
    }
}