using System.Device.Gpio;
using System.Net.Sockets;
using Franks.Reich.Performance.Ctrl.Services.GpioEventHandlerRegistryService;
using Franks.Reich.Performance.Ctrl.Services.GpioEventHandlerRegistryService.GpioEventHandlers;
using Franks.Reich.Performance.Ctrl.Services.GpioEventRoutingService;
using Franks.Reich.Performance.Ctrl.Services.GpioWatcherService;
using Franks.Reich.Performance.Ctrl.Services.OpenSoundControlService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

Console.WriteLine("Performance Ctrl");

var builder = new HostBuilder();

builder.ConfigureServices((_, services) =>
{
    services.AddLogging(loggingBuilder =>
    {
        loggingBuilder.AddConsole();
    });

    services.AddTransient<GpioController>();
    services.AddTransient<SelectChannelEventHandler>();
    services.AddTransient<UdpClient>(_ => new UdpClient("192.168.0.236", 1337));
    services.AddSingleton<IOpenSoundControlService, OpenSoundControlService>();
    services.AddSingleton<IGpioEventHandlerRegistry, GpioEventHandlerRegistry>();
    services.AddSingleton<IGpioEventRoutingService>(serviceProvider =>
    {
        (int, GpioEventRouteTarget)[] mappings =
        [
            (1, new GpioEventRouteTarget(EventHandlerType.SelectChannel, [1, 1])),
            (4, new GpioEventRouteTarget(EventHandlerType.SelectChannel, [1, 2])),
        ];

        var registry = serviceProvider.GetRequiredService<IGpioEventHandlerRegistry>();
        return new GpioEventRoutingService(
            registry, mappings.ToDictionary(
                x => x.Item1,
                y => y.Item2));
    });
    services.AddHostedService(serviceProvider =>
    {
        var routingService = serviceProvider.GetRequiredService<IGpioEventRoutingService>();
        var logger = serviceProvider.GetRequiredService<ILogger<GpioWatcherService>>();
        var gpioController = serviceProvider.GetRequiredService<GpioController>();
        return new GpioWatcherService(1, logger, routingService, gpioController);
    });
});

var app = builder.Build();
await app.RunAsync();