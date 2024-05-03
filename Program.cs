using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Trans;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((ctx, services) =>
    {
        string Get(string key) 
            => ctx.Configuration[key];

        var apiKey = Get("ApiKey");
        var region = Get("Region");
        var botKey = Get("Telegram");

        services.AddSingleton(_ =>
            new TranslationClient(apiKey, region));

        services.AddSingleton(_ => 
            new TelegramBotClient(botKey));

        services.AddSingleton(_ => new HttpClient());
    })
    .Build();


host.Run();