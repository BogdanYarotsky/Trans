using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Trans;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((ctx, services) =>
    {
        string Get(string key) 
            => ctx.Configuration[key];

        var apiKey = Get("ApiKey");
        var region = Get("Region");
        services.AddSingleton(
            new TranslationClient(apiKey, region));

    })
    .Build();


host.Run();