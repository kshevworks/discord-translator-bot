using System.Threading.Tasks;
using DiscordTranslatorBot.Types;

namespace DiscordTranslatorBot.Adapters.Database
{
    public interface IDatabaseClient
    {
        public Task InsertTranslationSettings(TranslationSettingsData data);
        public Task<TranslationSettingsData> GetTranslationSettingsData(ulong sourceChannelId);
        public Task<bool> RemoveTranslationSettings(ulong sourceChannelId);
    }
}