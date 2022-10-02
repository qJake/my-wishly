using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using MyWishly.App.Models;
using MyWishly.App.Models.Options;
using MyWishly.App.Models.Exceptions;

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
            TableClient = TableServiceClient.GetTableClient(Connections.TableNameUsers);
            TableClient.CreateIfNotExists();
        }

        public async Task<User> RegisterUser(User newUser)
        {
            // Data cleanup / normalization
            newUser.CreatedUtc = DateTimeOffset.Now;
            newUser.RowKey = newUser.RowKey!.Trim().ToLower();
            newUser.IsVerified = false;
            newUser.UserId = Guid.NewGuid();

            await TableClient.AddEntityAsync(newUser);

            return newUser;
        }

        public async Task<User> GetUser(string email)
        {
            try
            {
                var resp = await TableClient.GetEntityAsync<User>(User.PARTITION_KEY, email.ToLower().Trim());
                return resp.Value ?? throw new UserNotFoundException(email);
            }
            catch (RequestFailedException rex) when (rex.Status == 404)
            {
                throw new UserNotFoundException(email);
            }
        }

        public async Task<User> UpdateUser(User user)
        {
            try
            {
                var resp = await TableClient.UpdateEntityAsync(user, ETag.All, TableUpdateMode.Replace);
                return user;
            }
            catch (RequestFailedException rex) when (rex.Status == 404)
            {
                throw new UserNotFoundException(user.Email!);
            }
        }

        public async Task<User> GetUser(Guid userId)
        {
            try
            {
                var resp = await TableClient.QueryAsync<User>(filter: TableClient.CreateQueryFilter($"PartitionKey eq {User.PARTITION_KEY} and UserId eq {userId}"), maxPerPage: 1).FirstOrDefaultAsync();
                return resp ?? throw new UserNotFoundException(userId.ToString());
            }
            catch (RequestFailedException rex) when (rex.Status == 404)
            {
                throw new UserNotFoundException(userId.ToString());
            }
        }
    }
}
