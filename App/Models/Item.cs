using Azure;
using Azure.Data.Tables;

namespace MyWishly.App.Models
{
    public class Item : ITableEntity
    {
        public string? PartitionKey { get; set; }
        public string? RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
        
        public Guid UserId { get => PartitionKey is null ? Guid.Empty : Guid.Parse(PartitionKey); set => PartitionKey = value.ToString(); }
        public Guid ItemId { get => RowKey is null ? Guid.Empty : Guid.Parse(RowKey); set => RowKey = value.ToString(); }
        public string? Name { get; set; }
        public double Price { get; set; }
        public double PriceMax { get; set; }
        public bool IsHidden { get; set; }
        public bool IsBought { get; set; }
        public string? BoughtByIp { get; set; }
        public DateTimeOffset? BoughtTimeUtc { get; set; }
        public string? PrimaryBuyLink { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public DateTimeOffset CreatedUtc { get; set; }
        public DateTimeOffset UpdatedUtc { get; set; }
    }
}
