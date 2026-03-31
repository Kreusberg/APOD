using System.Text.Json.Nodes;

string tokenNasa = Environment.GetEnvironmentVariable("NASA_TOKEN");
string tokenBotTelegram = Environment.GetEnvironmentVariable("TELEGRAM_TOKEN");
string chatId = Environment.GetEnvironmentVariable("CHAT_ID_TOKEN");

HttpClient http = new();

string UrlAPOD = $"https://api.nasa.gov/planetary/apod?api_key={tokenNasa}";

string response = await http.GetStringAsync(UrlAPOD);

JsonNode jsonString = JsonNode.Parse(response);

string explanation = jsonString["explanation"]?.ToString();
string url = jsonString["url"]?.ToString();

var urlTelegram = $"https://api.telegram.org/bot{tokenBotTelegram}/sendPhoto";

FormUrlEncodedContent content = new(new[]
{
    new KeyValuePair<string, string>("chat_id", chatId),
    new KeyValuePair<string, string>("photo", url), 
    new KeyValuePair<string, string>("caption", explanation), 
});

await http.PostAsync(urlTelegram, content);
