using System;
using System.Threading.Tasks;
using DiscordTranslatorBot.Exceptions;
using DiscordTranslatorBot.Types;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DiscordTranslatorBot.Adapters.Database.Implementations
{
    public class MongoDatabaseClient : IDatabaseClient
    {
        private const string _settingsCollection = "settings";
        
        private const string _mongoAddress = "MONGO_ADDRESS";
        private const string _mongoDb = "MONGO_DB";
        private const string _mongoUsername = "MONGO_USERNAME";
        private const string _mongoPassword = "MONGO_PASSWORD";

        private readonly IMongoClient _client;
        private readonly IMongoDatabase _db;


        public MongoDatabaseClient()
        {
            var address = Environment.GetEnvironmentVariable(_mongoAddress);
            if (string.IsNullOrEmpty(address))
                throw new MissingEnvironmentVariableException(_mongoAddress);
            
            var db = Environment.GetEnvironmentVariable(_mongoDb);
            if (string.IsNullOrEmpty(db))
                throw new MissingEnvironmentVariableException(_mongoDb);

            var username = Environment.GetEnvironmentVariable(_mongoUsername);
            if (string.IsNullOrEmpty(username))
                throw new MissingEnvironmentVariableException(_mongoUsername);

            var password = Environment.GetEnvironmentVariable(_mongoPassword);
            if (string.IsNullOrEmpty(db))
                throw new MissingEnvironmentVariableException(_mongoPassword);

            var settings = new MongoClientSettings
            {
                Server = MongoServerAddress.Parse(address),
                Credential = MongoCredential.CreateCredential(db, username, password)
            };
            _client = new MongoClient(settings);

            _db = _client.GetDatabase(db);
        }

        private async Task<IMongoCollection<T>> GetCollection<T>(string name)
        {
            var collection = _db.GetCollection<T>(name);

            if (collection != null)
                return collection;

            await _db.CreateCollectionAsync(name);
            collection = _db.GetCollection<T>(name);
            return collection;
        }

        public async Task InsertTranslationSettings(TranslationSettingsData data)
        {
            var filter = GetSourceChannelIdFilter(data.sourceChannelId);
            var collection = await GetCollection<TranslationSettingsData>(_settingsCollection);
            if (await GetTranslationSettingsData(data.sourceChannelId) != null)
            {
                var document = data.ToBsonDocument();
                await collection.FindOneAndUpdateAsync(filter, document);
            }
            else
                await collection.InsertOneAsync(data);
        }

        public async Task<TranslationSettingsData> GetTranslationSettingsData(ulong sourceChannelId)
        {
            var filter = GetSourceChannelIdFilter(sourceChannelId);
            var collection = await GetCollection<TranslationSettingsData>(_settingsCollection);
            var documents = await collection.Find(filter).ToListAsync();
            if (documents == null || documents.Count == 0)
                return null;

            return documents[0];
        }

        public async Task<bool> RemoveTranslationSettings(ulong sourceChannelId)
        {
            var filter = GetSourceChannelIdFilter(sourceChannelId);
            var collection = await GetCollection<TranslationSettingsData>(_settingsCollection);
            var result = await collection.DeleteOneAsync(filter);
            return result.DeletedCount > 0;
        }

        private FilterDefinition<TranslationSettingsData> GetSourceChannelIdFilter(ulong sourceChannelId)
            => Builders<TranslationSettingsData>.Filter.Eq(x => x.sourceChannelId, sourceChannelId);
    }
}