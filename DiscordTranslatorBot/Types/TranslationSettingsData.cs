using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DiscordTranslatorBot.Types
{
    public class TranslationSettingsData
    {
        [BsonId]
        public ObjectId id;

        public ulong sourceChannelId;
        public ulong targetChannelId;
        public string sourceLanguageCode;
        public string targetLanguageCode;
    }
}