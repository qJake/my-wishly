using MyWishly.App.Models;

namespace MyWishly.App.Services
{
    public interface ISettingsService
    {
        Task<Settings?> GetFriendlyUrl(string urlSegment);
        Task<Settings?> GetSettings(Guid userId);
        Task<Settings> UpsertSettings(Settings settings);
    }
}