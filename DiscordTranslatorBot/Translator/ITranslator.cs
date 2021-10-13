using System.Threading.Tasks;
using Google.Cloud.Translation.V2;

namespace DiscordTranslatorBot.Translator
{
    public interface ITranslator
    {
        Task Init();
        Task<string> GetTranslation(string text, string sourceLanguageCode, string targetLanguageCode = LanguageCodes.English);
    }
}