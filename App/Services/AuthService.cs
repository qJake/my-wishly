using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using MyWishly.App.Models;
using MyWishly.App.Models.Options;
using MyWishly.App.Models.Exceptions;
using Azure;

namespace MyWishly.App.Services
{
    public class AuthService : IAuthService
    {
        public Connections Connections { get; }
        public TableServiceClient TableServiceClient { get; }
        public TableClient TableClient { get; }

        public AuthService(IOptions<Connections> connections)
        {
            Connections = connections.Value;
            TableServiceClient = new TableServiceClient(Connections.Storage);
            TableClient = TableServiceClient.GetTableClient(Connections.StorageTableName);
            TableClient.CreateIfNotExists();
        }

        public async Task RegisterUser(User newUser)
        {
            // Data cleanup / normalization
            newUser.CreatedUtc = DateTimeOffset.Now;
            newUser.RowKey = newUser.RowKey!.Trim().ToLower();
            newUser.IsVerified = false;

            await TableClient.AddEntityAsync(newUser);
        }

        public async Task<User> GetUser(string email)
        {
            try
            {
                var resp = await TableClient.GetEntityAsync<User>(User.PARTITION_KEY, email.ToLower().Trim());
                return resp.Value != null ? resp.Value : throw new UserNotFoundException(email);
            }
            catch (RequestFailedException rex) when (rex.Status == 404)
            {
                throw new UserNotFoundException(email);
            }
        }
    }
}
