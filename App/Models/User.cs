using Azure;
using Azure.Data.Tables;

namespace MyWishly.App.Models
{
    public class User : ITableEntity
    {
        public const string PARTITION_KEY = "Users";

        public string PartitionKey { get; set; } = PARTITION_KEY;
        public string? RowKey { get; set; }

        public string? Name { get; set; }
        public Guid UserId { get; set; }
        public string? FriendlyUrl { get; set; }
        public string? Email { get => RowKey; set => RowKey = value; }
        public string? PasswordHash { get; set; }
        public bool IsVerified { get; set; }
        public DateTimeOffset CreatedUtc { get; set; }
        public string? CreatedIpAddress { get; set; }

        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
