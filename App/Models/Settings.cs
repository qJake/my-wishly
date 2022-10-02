using Azure;
using Azure.Data.Tables;

namespace MyWishly.App.Models
{
    public class Settings : ITableEntity
    {
        public const string PARTITION_KEY = "Settings";

        public string PartitionKey { get; set; } = PARTITION_KEY;
        public string? RowKey { get; set; }

        public Guid UserId { get => Guid.Parse(RowKey ?? ""); set => RowKey = value.ToString(); }
        public string? FriendlyUrl { get; set; }

        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
