using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyWishly.App.Models;
using MyWishly.App.Services;
using MyWishly.App.ViewModels;
using System.Security.Claims;

namespace MyWishly.App.Controllers
{
    [Authorize]
    public class ItemsController : Controller
    {
        public IItemsService ItemsService { get; }
        public IImageService ImageService { get; }

        public ItemsController(IItemsService itemsService, IImageService imageService)
        {
            ItemsService = itemsService;
            ImageService = imageService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var items = await ItemsService.GetItemsForUser(Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            if (TempData["NewItem"] is string s)
            {
                ViewBag.NewItemName = s;
            }
            if (TempData["UpdatedItem"] is string u)
            {
                ViewBag.UpdatedItemName = u;
            }
            if (TempData["ItemNotFound"] is bool b && b)
            {
                ViewBag.ItemNotFound = true;
            }
            if (TempData["DeleteSuccess"] is bool b2 && b2)
            {
                ViewBag.DeleteSuccess = true;
            }
            if (TempData["DeleteFailed"] is bool b3 && b3)
            {
                ViewBag.DeleteFailed = true;
            }
            return View(items);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid itemId)
        {
            try
            {
                var item = await ItemsService.GetItem(Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)), itemId);
                ViewBag.ItemId = item.ItemId;
                return View(new ItemViewModel
                {
                    Name = item.Name,
                    Description = item.Description,
                    PurchaseUrl = item.PrimaryBuyLink,
                    Price = item.Price,
                    PreviousImageUrl = item.ImageUrl,
                    PriceMax = item.PriceMax,
                    IsHidden = item.IsHidden
                });
            }
            catch
            {
                TempData["ItemNotFound"] = true;
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid itemId)
        {
            try
            {
                await ItemsService.DeleteItem(Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)), itemId);
                TempData["DeleteSuccess"] = true;
            }
            catch
            {
                TempData["DeleteFailed"] = true;
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm] ItemViewModel item, [FromQuery] Guid itemId)
        {
            if (ModelState.IsValid)
            {
                var existingItem = await ItemsService.GetItem(Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)), itemId);

                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                
                string? newUrl = existingItem.ImageUrl;
                if (item.Image is not null)
                {
                    using (var ms = new MemoryStream())
                    {
                        await item.Image.CopyToAsync(ms);
                        newUrl = await ImageService.UploadProductImage(userId, ms.ToArray(), Path.GetExtension(item.Image.FileName));
                    }
                }

                existingItem.Name = item.Name;
                existingItem.Description = item.Description;
                existingItem.Price = item.Price;
                existingItem.PrimaryBuyLink = item.PurchaseUrl;
                existingItem.UpdatedUtc = DateTimeOffset.UtcNow;
                existingItem.ImageUrl = newUrl;
                existingItem.PriceMax = item.PriceMax;
                existingItem.IsHidden = item.IsHidden;
                
                await ItemsService.UpdateItem(existingItem);

                TempData["UpdatedItem"] = item.Name;

                return RedirectToAction("Index");
            }
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ItemViewModel newItem)
        {
            if (ModelState.IsValid)
            {
                if (newItem.Image is null)
                {
                    ModelState.AddModelError(nameof(newItem.Image), "Product image is required.");
                    return View(newItem);
                }

                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                string url;
                using (var ms = new MemoryStream())
                { 
                    await newItem.Image.CopyToAsync(ms);
                    url = await ImageService.UploadProductImage(userId, ms.ToArray(), Path.GetExtension(newItem.Image.FileName));
                }
                var item = new Item
                {
                    CreatedUtc = DateTimeOffset.UtcNow,
                    UpdatedUtc = DateTimeOffset.UtcNow,
                    Description = newItem.Description,
                    Name = newItem.Name,
                    Price = newItem.Price,
                    PriceMax = newItem.PriceMax,
                    UserId = userId,
                    PrimaryBuyLink = newItem.PurchaseUrl,
                    ImageUrl = url,
                    IsHidden = newItem.IsHidden
                };

                await ItemsService.CreateItem(item);

                TempData["NewItem"] = item.Name;

                return RedirectToAction("Index");
            }
            return View(newItem);
        }
    }
}
