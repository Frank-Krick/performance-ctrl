using System.Device.Gpio;
using System.Net.Sockets;
using Franks.Reich.Performance.Ctrl.Configuration;
using Franks.Reich.Performance.Ctrl.Services.GpioEventHandlerRegistryService;
using Franks.Reich.Performance.Ctrl.Services.GpioEventHandlerRegistryService.GpioEventHandlers;
using Franks.Reich.Performance.Ctrl.Services.GpioEventRoutingService;
using Franks.Reich.Performance.Ctrl.Services.GpioWatcherService;
using Franks.Reich.Performance.Ctrl.Services.OpenSoundControlService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

Console.WriteLine("Performance Ctrl");

var builder = new HostBuilder();

builder.ConfigureAppConfiguration((context, configBuilder) =>
{
    configBuilder.AddJsonFile("./appsettings.json");
});
    
builder.ConfigureServices((context, services) =>
{
    services.Configure<List<GpioMappingEntry>>(
        context.Configuration.GetRequiredSection("GpioMappings"));
    
    services.AddLogging(loggingBuilder =>
    {
        loggingBuilder.AddConsole();
    });

    services.AddSingleton<GpioController>();
    services.AddTransient<SelectChannelEventHandler>();
    services.AddTransient<UdpClient>(_ => new UdpClient("192.168.0.236", 1337));
    services.AddSingleton<IOpenSoundControlService, OpenSoundControlService>();
    services.AddSingleton<IGpioEventHandlerRegistry, GpioEventHandlerRegistry>();
    services.AddSingleton<IGpioEventRoutingService>(serviceProvider =>
    {
        var options = serviceProvider.GetRequiredService<IOptions<List<GpioMappingEntry>>>();
        var registry = serviceProvider.GetRequiredService<IGpioEventHandlerRegistry>();
        var logger = serviceProvider.GetRequiredService<ILogger<GpioEventRoutingService>>();
        
        var mappings = options.Value
            .Select(o => (o.PinId, new GpioEventRouteTarget(
                EventHandlerType.SelectChannel, o.Parameters)));
        
        return new GpioEventRoutingService(
            registry, mappings.ToDictionary(
                x => x.Item1,
                y => y.Item2),
            logger);
    });
    
    var gpioMappings = context
        .Configuration
        .GetRequiredSection("GpioMappings")
        .Get<List<GpioMappingEntry>>();

    foreach (var mapping in gpioMappings!)
    {
        var serviceProvider = services.BuildServiceProvider();
        var routingService = serviceProvider.GetRequiredService<IGpioEventRoutingService>();
        var logger = serviceProvider.GetRequiredService<ILogger<GpioWatcherService>>();
        var gpioController = serviceProvider.GetRequiredService<GpioController>();
        var service = new GpioWatcherService(mapping.PinId, logger, routingService, gpioController);
        _ = service.StartAsync(CancellationToken.None);
        services.AddSingleton(service);
    }
});

var app = builder.Build();
await app.RunAsync();