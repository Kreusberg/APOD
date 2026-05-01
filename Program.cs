using APOD;
using System.Text.Json.Nodes;

string response = await Util.GetApodResponse();

JsonNode jsonString = JsonNode.Parse(response) ?? "";

string url = jsonString["url"]?.ToString() ?? "";
string explanation = jsonString["explanation"]?.ToString() ?? "";

string explanationPtBr = await Util.GetTranslatedExplanation(explanation);

await Util.SendTelegramMessage(url, explanationPtBr);
