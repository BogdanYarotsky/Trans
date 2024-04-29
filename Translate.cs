using System.Globalization;
using System.Net;
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
            [HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            var resp = req.CreateResponse(HttpStatusCode.OK);
            resp.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            try
            {
                var translation = await _client.GermanToEnglishAsync("Apfel", default);
                await resp.WriteStringAsync($"Welcome to {translation} Functions!");
            }
            catch (Exception e)
            {
                await resp.WriteStringAsync(e.ToString());
            }

            return new MultiResponse
            {
                HttpResponse = resp,
                Document = new MyDocument
                {
                    partitionKey = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                    id = "Gnida",
                }
            };
        }
    }
}
