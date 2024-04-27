using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Trans
{
    public class Translate
    {
        private readonly ILogger<Translate> _logger;
        private readonly AzureTranslationClient _client;

        public Translate(ILogger<Translate> logger, AzureTranslationClient client)
        {
            _logger = logger;
            _client = client;
        }

        [Function("Translate")]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var resp = req.CreateResponse(HttpStatusCode.OK);
            resp.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            var translation = await _client.GermanToEnglishAsync("Apfel", default);
            await resp.WriteStringAsync($"Welcome to {translation} Functions!");

            return resp;
        }
    }
}
