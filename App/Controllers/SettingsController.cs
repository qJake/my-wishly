using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyWishly.App.Services;
using MyWishly.App.ViewModels;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyWishly.App.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        public ISettingsService SettingsService { get; }

        public SettingsController(ISettingsService settingsService)
        {
            SettingsService = settingsService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var settings = await SettingsService.GetSettings(Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)))
                ?? new Models.Settings();
            if (TempData["SettingsSaved"] is bool b && b)
            {
                ViewBag.SettingsSaved = true;
            }
            return View(new SettingsViewModel
            {
                FriendlyUrl = settings.FriendlyUrl
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(SettingsViewModel newSettings)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ModelState.IsValid)
            {
                var settings = await SettingsService.GetSettings(Guid.Parse(userId))
                    ?? new Models.Settings();

                if (!string.IsNullOrWhiteSpace(newSettings.FriendlyUrl))
                {
                    var existingUrl = await SettingsService.GetFriendlyUrl(newSettings.FriendlyUrl);
                    if (existingUrl is not null && existingUrl.RowKey != userId)
                    {
                        ModelState.AddModelError(nameof(SettingsViewModel.FriendlyUrl), "That URL is already taken, please choose a different one!");
                        return View(nameof(Index));
                    }

                    settings.FriendlyUrl = newSettings.FriendlyUrl;
                }

                settings.RowKey = userId;

                await SettingsService.UpsertSettings(settings);
                TempData["SettingsSaved"] = true;
                return RedirectToAction(nameof(Index));
            }
            return View(newSettings);
        }
    }
}
