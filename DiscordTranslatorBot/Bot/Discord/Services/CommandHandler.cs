using System.Reflection;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using DiscordTranslatorBot.Adapters.Database;

namespace DiscordTranslatorBot.Bot.Discord.Services
{
    public class CommandHandler
    {
        private const char _prefix = '~';
        
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly IDatabaseClient _databaseClient;

        public CommandHandler(DiscordSocketClient client, CommandService commandService, IDatabaseClient databaseClient)
        {
            _client = client;
            _commandService = commandService;
            _databaseClient = databaseClient;
            _client.MessageReceived += HandleCommandAsync;
        }

        public async Task InstallCommandsAsync()
        {
            await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), null);
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            if (messageParam is not SocketUserMessage message) 
                return;

            var argPos = 0;

            if (!(message.HasCharPrefix(_prefix, ref argPos) ||
                  message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                return;

            var context = new CustomSocketCommandContext(_client, message, _databaseClient);

            await _commandService.ExecuteAsync(context, argPos, null);
        }

        ~CommandHandler()
        {
            _client.MessageReceived -= HandleCommandAsync;
        }
    }
}