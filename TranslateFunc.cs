using System.Globalization;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

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

    public class TranslateFunc
    {
        private readonly ILogger<TranslateFunc> _logger;
        private readonly TranslationClient _translator;
        private readonly TelegramBotClient _telegramBot;

        public TranslateFunc(
            ILogger<TranslateFunc> logger, 
            TranslationClient translator,
            TelegramBotClient telegramBot)
        {
            _logger = logger;
            _translator = translator;
            _telegramBot = telegramBot;
        }

        [Function("Translate")]
        public async Task<MultiResponse> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            var d = new MyDocument
            {
                partitionKey = Guid.NewGuid().ToString(),
                id = "Gnida"
            };

            try
            {
                using var reader = new StreamReader(req.Body);
                var body = await reader.ReadToEndAsync();
                var update = JsonSerializer.Deserialize<TelegramUpdate>(body);
                var message = update?.Message.Text ?? string.Empty;
                var chatId = update?.Message.Chat.Id ?? throw new Exception("");
                await Task.Delay(TimeSpan.FromSeconds(1));
                await _telegramBot.SendTextMessageAsync(chatId, $"{message}??? Пішов нахуй!");
                d.lobzik = message;
            }
            catch (Exception e)
            {
                d.lobzik = e.ToString();
            }

            return new MultiResponse
            {
                HttpResponse = req.CreateResponse(HttpStatusCode.OK),
                Document = d
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
