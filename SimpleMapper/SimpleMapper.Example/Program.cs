using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimpleMapper.Example;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
    {
        var startup = new Startup();
        startup.ConfigureServices(services);
    })
    .Build();

var app = host.Services.GetRequiredService<App>();
app.Run();