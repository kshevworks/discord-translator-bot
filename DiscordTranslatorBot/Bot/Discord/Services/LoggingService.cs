using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DiscordTranslatorBot.Bot.Discord.Services
{
    public class LoggingService
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        
        public LoggingService(DiscordSocketClient client, CommandService commandService)
        {
            _client = client;
            _commandService = commandService;

            _client.Log += LogAsync;
            _commandService.Log += LogAsync;
        }
        
        private Task LogAsync(LogMessage message)
        {
            if (message.Exception is CommandException cmdException)
            {
                Console.WriteLine($"[Command/{message.Severity}] {cmdException.Command.Aliases.First()}"
                                  + $" failed to execute in {cmdException.Context.Channel}.");
                Console.WriteLine(cmdException);
            }
            else 
                Console.WriteLine($"[General/{message.Severity}] {message}");
            
            return Task.CompletedTask;
        }
        
        ~LoggingService()
        {
            _client.Log -= LogAsync;
            _commandService.Log -= LogAsync;
        }
    }
}