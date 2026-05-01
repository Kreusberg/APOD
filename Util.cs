using System.Text.Json;
using System.Text.Json.Nodes;

namespace APOD
{
    public class Util
    {
        private static readonly HttpClient http = new();


        public static async Task<string> GetApodResponse()
        {
            string tokenNasa = Environment.GetEnvironmentVariable("NASA_TOKEN") ?? "";

            string UrlAPOD = $"https://api.nasa.gov/planetary/apod?api_key={tokenNasa}";
            return await http.GetStringAsync(UrlAPOD);
        }

        public static async Task SendTelegramMessage(string url, string explanation)
        {
            string tokenBotTelegram = Environment.GetEnvironmentVariable("TELEGRAM_TOKEN") ?? "";
            string chatId = Environment.GetEnvironmentVariable("CHAT_ID_TOKEN") ?? "";

            bool isVideo = url.Contains("mp4");
            string method = isVideo ? "sendVideo" : "sendPhoto";

            var urlTelegram = $"https://api.telegram.org/bot{tokenBotTelegram}/{method}";

            FormUrlEncodedContent content = new(new[]
            {
                new KeyValuePair<string, string>("chat_id", chatId),
                new KeyValuePair<string, string>(isVideo ? "video" : "photo", url),
                new KeyValuePair<string, string>("caption", explanation),
            });

            var aa = await http.PostAsync(urlTelegram, content);
        }

        public static async Task<string> GetTranslatedExplanation(string explanation)
        {
            string googleAiToken = Environment.GetEnvironmentVariable("GOOGLE_AI_TOKEN") ?? "";
            string UrlGoogleAI = $"https://generativelanguage.googleapis.com/v1/models/gemini-2.5-flash:generateContent?key={googleAiToken}";

            var obj = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new
                            {
                               text = $"Resuma o texto abaixo em português (Brasil), mantendo o significado original.\n\n" +
                                $"- Responda apenas com o resumo final.\n" +
                                $"- NÃO inclua a tradução completa.\n" +
                                $"- NÃO inclua títulos ou prefixos como 'Resumo:'.\n" +
                                $"- O resultado deve ter no máximo 1000 caracteres.\n\n" +
                                $"Texto:\n{explanation}"
                            }
                        }
                    }
                }
            };

            StringContent googleJson = new(JsonSerializer.Serialize(get), System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage responseGoogleAI = await http.PostAsync(UrlGoogleAI, googleJson);

            JsonNode resposeGoogleJN = JsonNode.Parse(await responseGoogleAI.Content.ReadAsStringAsync()) ?? "";

            return resposeGoogleJN["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString() ?? "";

        }
    }
}
