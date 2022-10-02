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

        public DashboardController(IItemsService itemsService)
        {
            ItemsService = itemsService;
        }

        public async Task<IActionResult> Index()
        {
            var items = await ItemsService.GetItemsForUser(Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            return View(new DashboardViewModel
            {
                ItemCount = items.Count()
            });
        }
    }
}
