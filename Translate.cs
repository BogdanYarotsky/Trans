using System.Globalization;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Trans
{
    public class MultiResponse
    {
        [CosmosDBOutput("ToDoList", "Items",
            Connection = "CosmosDbConnectionSetting")]
        public MyDocument Document { get; set; }
        public HttpResponseData HttpResponse { get; set; }
    }

    public class MyDocument
    {
        public string partitionKey { get; set; }
        public string id { get; set; }
        public string lobzik { get; set; }
    }

    public class Translate
    {
        private readonly ILogger<Translate> _logger;
        private readonly TranslationClient _client;

        public Translate(ILogger<Translate> logger, TranslationClient client)
        {
            _logger = logger;
            _client = client;
        }

        [Function("Translate")]
        public async Task<MultiResponse> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            var resp = req.CreateResponse(HttpStatusCode.OK);
            resp.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            using var reader = new StreamReader(req.Body);
            var body = await reader.ReadToEndAsync();
            var update = JsonSerializer.Deserialize<TelegramUpdate>(body);

            return new MultiResponse
            {
                HttpResponse = resp,
                Document = new MyDocument
                {
                    partitionKey = Guid.NewGuid().ToString(),
                    lobzik = update?.Message.Text ?? string.Empty,
                }
            };
        }
    }

    public class Chat
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string LastName { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }

    public class From
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("is_bot")]
        public bool IsBot { get; set; }

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string LastName { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("language_code")]
        public string LanguageCode { get; set; }
    }

    public class Message
    {
        [JsonPropertyName("message_id")]
        public int MessageId { get; set; }

        [JsonPropertyName("from")]
        public From From { get; set; }

        [JsonPropertyName("chat")]
        public Chat Chat { get; set; }

        [JsonPropertyName("date")]
        public int Date { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }
    }

    public class TelegramUpdate
    {
        [JsonPropertyName("update_id")]
        public int UpdateId { get; set; }

        [JsonPropertyName("message")]
        public Message Message { get; set; }
    }


}
