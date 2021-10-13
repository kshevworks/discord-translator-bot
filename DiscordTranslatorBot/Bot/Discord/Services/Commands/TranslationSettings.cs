using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordTranslatorBot.Types;

namespace DiscordTranslatorBot.Bot.Discord.Services.Commands
{
    public class TranslationSettings : ModuleBase<CustomSocketCommandContext>
    {
        private ulong GetChannelId(string channelName) => Context.Guild.TextChannels.First(x => x.Name == channelName).Id;
        
        [Command("set-translation")]
        public async Task SetTranslation(string sourceChannel, string targetChannel, string sourceLanguageCode, string targetLanguageCode)
        {
            var user = Context.User as IGuildUser;
            if (user == null || !user.GuildPermissions.Administrator)
                return;
            
            var sourceChannelId = GetChannelId(sourceChannel);
            var targetChannelId = GetChannelId(targetChannel);
            
            var translationSettingsData = new TranslationSettingsData
            {
                sourceChannelId = sourceChannelId,
                targetChannelId = targetChannelId,
                sourceLanguageCode = sourceLanguageCode,
                targetLanguageCode = targetLanguageCode,
            };

            await Context.DatabaseClient.InsertTranslationSettings(translationSettingsData);
            await Context.Channel.SendMessageAsync($"Messages from channel {sourceChannel} will translate from {sourceLanguageCode} to {targetLanguageCode} and will be posted in {targetChannel} channel.");
        }

        [Command("remove-translation")]
        public async Task RemoveTranslation(string sourceChannel)
        {
            var user = Context.User as IGuildUser;
            if (user == null || !user.GuildPermissions.Administrator)
                return;

            var sourceChannelId = GetChannelId(sourceChannel);
            var deleted = await Context.DatabaseClient.RemoveTranslationSettings(sourceChannelId);
            if (deleted)
                await Context.Channel.SendMessageAsync($"Messages from channel {sourceChannel} now won't be translated.");
            else
                await Context.Channel.SendMessageAsync($"Rule for {sourceChannel} not found");
        }
    }
}