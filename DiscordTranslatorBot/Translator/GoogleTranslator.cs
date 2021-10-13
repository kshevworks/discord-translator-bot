using System;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Translation.V2;

namespace DiscordTranslatorBot.Translator
{
    public class GoogleTranslator : ITranslator
    {
        private const string _keyName = "key.json";
        private TranslationClient _client;

        public async Task Init()
        {
            var credential = Authorize();
            if (credential == null)
            {
                Console.WriteLine("Failed to authorize Google Cloud Platform.");
                return;
            }

            _client = await TranslationClient.CreateAsync(credential);
        }

        private GoogleCredential Authorize()
        {
            var credential = GoogleCredential.FromFile(_keyName);
            return credential;
        }

        public async Task<string> GetTranslation(string text, string sourceLanguageCode, string targetLanguageCode = LanguageCodes.English)
        {
            var result = await _client.TranslateTextAsync(text, targetLanguageCode, sourceLanguageCode);
            return result.TranslatedText;
        }
    }
}