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

        services.AddSingleton(() =>
            new TranslationClient(apiKey, region));

        services.AddSingleton(() => 
            new TelegramBotClient(botKey));
    })
    .Build();


host.Run();