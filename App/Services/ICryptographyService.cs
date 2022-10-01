namespace MyWishly.App.Services
{
    public interface ICryptographyService
    {
        Task<bool> CheckPassword(string hashedPw, string testPassword, DateTimeOffset created);
        Task<string> HashPassword(string password, DateTimeOffset created);
    }
}