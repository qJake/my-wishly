using MyWishly.App.Models;

namespace MyWishly.App.Services
{
    public interface IItemsService
    {
        Task<Item> CreateItem(Item newItem);
        Task DeleteItem(Guid userId, Guid itemId);
        Task<Item> GetItem(Guid userId, Guid itemId);
        Task<IEnumerable<Item>> GetItemsForUser(Guid userId);
        Task<Item> UpdateItem(Item item);
    }
}