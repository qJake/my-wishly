using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyWishly.App.Services;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyWishly.App.Controllers
{
    [Authorize]
    public class ArrangeController : Controller
    {
        public IItemsService ItemsService { get; }
        public IImageService ImageService { get; }

        public ArrangeController(IItemsService itemsService, IImageService imageService)
        {
            ItemsService = itemsService;
            ImageService = imageService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var items = await ItemsService.GetItemsForUser(Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            if (TempData["OrderSaved"] is bool b && b)
            {
                ViewBag.OrderSaved = true;
            }
            return View(items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveOrder([FromForm] string newOrder)
        {
            var newOrderList = JsonSerializer.Deserialize<List<Guid>>(newOrder);
            if (newOrderList is null)
            {
                return RedirectToAction(nameof(Index));
            }
            var items = await ItemsService.GetItemsForUser(Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            for (var i = 0; i < newOrderList.Count; i++)
            {
                var item = items.FirstOrDefault(it => it.ItemId == newOrderList[i]);
                if (item is null)
                {
                    continue;
                }
                item.Order = i;
                await ItemsService.UpdateItem(item);
            }
            TempData["OrderSaved"] = true;
            return RedirectToAction(nameof(Index));
        }
    }
}
