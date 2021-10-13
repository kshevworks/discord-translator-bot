using System.Threading.Tasks;
using DiscordTranslatorBot.Adapters.Database;
using DiscordTranslatorBot.Adapters.Database.Implementations;
using DiscordTranslatorBot.Bot;
using DiscordTranslatorBot.Bot.Discord;
using DiscordTranslatorBot.Translator;

namespace DiscordTranslatorBot
{
    public class Program
    {
        private ITranslator _translator;
        private IBot _bot;
        private IDatabaseClient _databaseClient;
        
        public static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        private async Task MainAsync()
        {
            _databaseClient = new MongoDatabaseClient();
            
            _translator = new GoogleTranslator();
            await _translator.Init();
            
            _bot = new DiscordBot(_translator, _databaseClient);
            await _bot.Start();

            await Task.Delay(-1);
        }
    }
}
