using System.Text.Json;
using System.Text.Json.Nodes;

string tokenNasa = Environment.GetEnvironmentVariable("NASA_TOKEN") ?? "";
string tokenBotTelegram = Environment.GetEnvironmentVariable("TELEGRAM_TOKEN") ?? "";
string chatId = Environment.GetEnvironmentVariable("CHAT_ID_TOKEN") ?? "";
string googleAiToken = Environment.GetEnvironmentVariable("GOOGLE_AI_TOKEN") ?? "";

HttpClient http = new();

string UrlAPOD = $"https://api.nasa.gov/planetary/apod?api_key={tokenNasa}";
string UrlGoogleAI = $"https://generativelanguage.googleapis.com/v1/models/gemini-2.5-flash:generateContent?key={googleAiToken}";

string response = await http.GetStringAsync(UrlAPOD);

JsonNode jsonString = JsonNode.Parse(response) ?? "";

string explanation = jsonString["explanation"]?.ToString() ?? "";
string url = jsonString["url"]?.ToString() ?? "";

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
                    text = $"{explanation} - Dado este texto, traduza-o para português (brasileiro). Sua resposta deve conter somente a tradução, sem demais comentários."
                }
            }
        }
    }
};

StringContent googleJson = new(JsonSerializer.Serialize(obj), System.Text.Encoding.UTF8, "application/json");

HttpResponseMessage responseGoogleAI = await http.PostAsync(UrlGoogleAI, googleJson);

JsonNode resposeGoogleJN = JsonNode.Parse(await responseGoogleAI.Content.ReadAsStringAsync()) ?? "";

var explanationPtBr = resposeGoogleJN["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString() ?? "";

var urlTelegram = $"https://api.telegram.org/bot{tokenBotTelegram}/sendPhoto";

FormUrlEncodedContent content = new(new[]
{
    new KeyValuePair<string, string>("chat_id", chatId),
    new KeyValuePair<string, string>("photo", url),
    new KeyValuePair<string, string>("caption", explanationPtBr),
});

await http.PostAsync(urlTelegram, content);
