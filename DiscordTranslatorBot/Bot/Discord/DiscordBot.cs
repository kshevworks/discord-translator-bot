using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordTranslatorBot.Adapters.Database;
using DiscordTranslatorBot.Bot.Discord.Services;
using DiscordTranslatorBot.Exceptions;
using DiscordTranslatorBot.Translator;

namespace DiscordTranslatorBot.Bot.Discord
{
    public class DiscordBot : IBot
    {
        private const string _discordToken = "DISCORD_TOKEN";
        private readonly ITranslator _translator;
        private readonly IDatabaseClient _databaseClient;

        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly LoggingService _loggingService;
        private readonly CommandHandler _commandHandler;


        public DiscordBot(ITranslator translator, IDatabaseClient databaseClient)
        {
            _translator = translator;
            _databaseClient = databaseClient;
            _client = new DiscordSocketClient();
            _commandService = new CommandService();
            _loggingService = new LoggingService(_client, _commandService);
            _commandHandler = new CommandHandler(_client, _commandService, _databaseClient);
        }

        public async Task Start()
        {
            _client.MessageReceived += ProcessMessage;
            await _commandHandler.InstallCommandsAsync();
            var token = Environment.GetEnvironmentVariable(_discordToken);
            if (string.IsNullOrEmpty(token))
                throw new MissingEnvironmentVariableException(_discordToken);

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
        }

        private async Task ProcessMessage(SocketMessage msg)
        {
            if (msg.Content == string.Empty)
                return;

            if (msg.Type != MessageType.Default || msg.Content.StartsWith('~'))
                return;

            await SendTranslatedMessage(msg);
        }

        private async Task SendTranslatedMessage(SocketMessage msg)
        {
            var settings = await _databaseClient.GetTranslationSettingsData(msg.Channel.Id);
            if (settings == null)
                return;

            var channel = await _client.GetChannelAsync(settings.targetChannelId) as IMessageChannel;
            if (channel == null)
                return;

            var builder = new EmbedBuilder();
            builder.WithAuthor(msg.Author);
            var translation = await _translator.GetTranslation(msg.Content, settings.sourceLanguageCode,
                settings.targetLanguageCode);

            builder.WithDescription(translation);
            builder.WithTimestamp(msg.Timestamp);
            builder.WithFooter($"Original: {msg.Content}");
            await channel.SendMessageAsync("", false, builder.Build());
        }
    }
}