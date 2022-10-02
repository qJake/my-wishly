using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using MyWishly.App.Models;
using MyWishly.App.Models.Options;
using MyWishly.App.Models.Exceptions;

namespace MyWishly.App.Services
{
    public class SettingsService : ISettingsService
    {
        public Connections Connections { get; }
        public TableServiceClient TableServiceClient { get; }
        public TableClient TableClient { get; }

        public SettingsService(IOptions<Connections> connections)
        {
            Connections = connections.Value;
            TableServiceClient = new TableServiceClient(Connections.Storage);
            TableClient = TableServiceClient.GetTableClient(Connections.TableNameSettings);
            TableClient.CreateIfNotExists();
        }

        public async Task<Settings?> GetSettings(Guid userId)
        {
            try
            {

                return await TableClient.GetEntityAsync<Settings>(Settings.PARTITION_KEY, userId.ToString());
            }
            catch (RequestFailedException rex) when (rex.Status == 404)
            {
                return null;
            }
        }

        public async Task<Settings> UpsertSettings(Settings settings)
        {
            // Data cleanup / normalization
            await TableClient.UpsertEntityAsync(settings, TableUpdateMode.Replace);

            return settings;
        }

        public async Task<Settings?> GetFriendlyUrl(string urlSegment)
        {
            try
            {
                return await TableClient.QueryAsync<Settings>(TableClient.CreateQueryFilter($"FriendlyUrl eq {urlSegment}")).FirstOrDefaultAsync();
            }
            catch (RequestFailedException rex) when (rex.Status == 404)
            {
                return null;
            }
        }
    }
}
