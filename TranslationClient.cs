using System.Text.Json;

namespace Trans;

using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;

public class TranslationClient : HttpClient
{
    public TranslationClient(string apiKey, string region)
    {
        DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
        DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Region", region);
    }

    public async Task<string?> EnglishToGermanAsync(string text, CancellationToken ct)
    {
        object[] body = { new { Text = text } };
        var requestBody = JsonSerializer.Serialize(body);
        using var request = new HttpRequestMessage();
        request.Method = HttpMethod.Post;

        request.RequestUri =
            new Uri("https://api.cognitive.microsofttranslator.com/translate?api-version=3.0&from=en&to=de");
        request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");

        var response = await SendAsync(request, ct);
        var content = await response.Content.ReadAsStringAsync(ct);

        return JsonSerializer
            .Deserialize<TranslationContent[]>(content)?
            .FirstOrDefault()?
            .Translations
            .FirstOrDefault()?
            .Text;
    }
}

public record TranslationContent(
    [property: JsonPropertyName("translations")] 
    Translation[] Translations
);

public record Translation(
    [property: JsonPropertyName("text")] 
    string Text
);