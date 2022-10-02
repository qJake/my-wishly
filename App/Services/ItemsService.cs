using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using MyWishly.App.Models;
using MyWishly.App.Models.Options;

namespace MyWishly.App.Services
{
    public class ItemsService : IItemsService
    {
        public Connections Connections { get; }
        public TableServiceClient TableServiceClient { get; }
        public TableClient TableClient { get; }
        public BlobServiceClient BlobServiceClient { get; }
        public BlobContainerClient ContainerClient { get; }

        public ItemsService(IOptions<Connections> connections)
        {
            Connections = connections.Value;
            TableServiceClient = new TableServiceClient(Connections.Storage);
            TableClient = TableServiceClient.GetTableClient(Connections.TableNameItems);
            TableClient.CreateIfNotExists();

            BlobServiceClient = new BlobServiceClient(Connections.Storage);
            ContainerClient = BlobServiceClient.GetBlobContainerClient(Connections.ContainerNameProductImages);
            ContainerClient.CreateIfNotExists();
        }

        public async Task<Item> CreateItem(Item newItem)
        {
            if (newItem.UserId == Guid.Empty)
            {
                throw new ArgumentException("UserId property cannot be empty.", nameof(Item.UserId));
            }
            
            newItem.ItemId = Guid.NewGuid();

            await TableClient.AddEntityAsync(newItem);

            return newItem;
        }

        public async Task<Item> UpdateItem(Item item)
        {
            if (item.ItemId == Guid.Empty)
            {
                throw new ArgumentException("ItemId property cannot be empty.", nameof(Item.ItemId));
            }
            if (item.UserId == Guid.Empty)
            {
                throw new ArgumentException("UserId property cannot be empty.", nameof(Item.UserId));
            }

            await TableClient.UpdateEntityAsync(item, Azure.ETag.All, TableUpdateMode.Replace);

            return item;
        }

        public async Task<Item> GetItem(Guid userId, Guid itemId)
        {
            var item = await TableClient.GetEntityAsync<Item>(userId.ToString(), itemId.ToString());
            return item;
        }

        public async Task DeleteItem(Guid userId, Guid itemId)
        {
            await TableClient.DeleteEntityAsync(userId.ToString(), itemId.ToString());
        }

        public async Task<IEnumerable<Item>> GetItemsForUser(Guid userId)
        {
            var pageResp = TableClient.QueryAsync<Item>(TableClient.CreateQueryFilter($"PartitionKey eq {userId.ToString()}"));
            List<Item> items = new();
            await foreach (var page in pageResp.AsPages())
            {
                items.AddRange(page.Values);
            }
            return items;
        }
    }
}
