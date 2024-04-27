using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Trans;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((ctx, services) =>
    {
        var apiKey = ctx.Configuration["ApiKey"];
        var region = ctx.Configuration["Region"];
        services.AddSingleton(new AzureTranslationClient(apiKey, region));
    })
    .Build();


host.Run();