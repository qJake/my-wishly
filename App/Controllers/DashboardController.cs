using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyWishly.App.Services;
using MyWishly.App.ViewModels;
using System.Security.Claims;

namespace MyWishly.App.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        public IItemsService ItemsService { get; }
        public ISettingsService SettingsService { get; }

        public DashboardController(IItemsService itemsService, ISettingsService settingsService)
        {
            ItemsService = itemsService;
            SettingsService = settingsService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var items = await ItemsService.GetItemsForUser(userId);
            var settings = await SettingsService.GetSettings(userId);
            return View(new DashboardViewModel
            {
                ItemCount = items.Count(),
                FriendlyUrl = !string.IsNullOrWhiteSpace(settings?.FriendlyUrl) ? settings?.FriendlyUrl : null
            });
        }
    }
}
