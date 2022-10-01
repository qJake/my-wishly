using Microsoft.Extensions.Options;
using MyWishly.App.Models.Options;
using System.Security.Cryptography;
using System.Text;

namespace MyWishly.App.Services
{
    public class CryptographyService : ICryptographyService
    {
        public Cryptography CryptoOptions { get; }

        public CryptographyService(IOptions<Cryptography> cryptoOptions)
        {
            CryptoOptions = cryptoOptions.Value;
        }

        public async Task<string> HashPassword(string password, DateTimeOffset created)
        {
            var pwBytes = Encoding.UTF8.GetBytes(password + CryptoOptions.Salt + created.ToUnixTimeSeconds());

            using var sha512 = SHA512.Create();
            var hash = await sha512.ComputeHashAsync(new MemoryStream(pwBytes));
            return Convert.ToHexString(hash);
        }

        public async Task<bool> CheckPassword(string hashedPw, string testPassword, DateTimeOffset created)
        {
            var pwBytes = Encoding.UTF8.GetBytes(testPassword + CryptoOptions.Salt + created.ToUnixTimeSeconds());

            using var sha512 = SHA512.Create();
            var hash = await sha512.ComputeHashAsync(new MemoryStream(pwBytes));
            return Convert.ToHexString(hash) == hashedPw;
        }
    }
}
