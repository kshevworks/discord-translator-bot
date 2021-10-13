using Discord.Commands;
using Discord.WebSocket;
using DiscordTranslatorBot.Adapters.Database;

namespace DiscordTranslatorBot.Bot.Discord.Services
{
    public class CustomSocketCommandContext : SocketCommandContext
    {
        public IDatabaseClient DatabaseClient { get; }

        public CustomSocketCommandContext(DiscordSocketClient client, SocketUserMessage msg, IDatabaseClient databaseClient) : base(client, msg)
        {
            DatabaseClient = databaseClient;
        }
    }
}