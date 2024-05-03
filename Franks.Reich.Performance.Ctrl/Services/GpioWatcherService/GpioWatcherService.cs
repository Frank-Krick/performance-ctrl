using System.Device.Gpio;
using Franks.Reich.Performance.Ctrl.Services.GpioEventRoutingService;
using Franks.Reich.Performance.Ctrl.Services.GpioEventRoutingService.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Franks.Reich.Performance.Ctrl.Services.GpioWatcherService;

public class GpioWatcherService(
    int pinId,
    ILogger<GpioWatcherService> logger,
    IGpioEventRoutingService routingService,
    GpioController gpioController) : IHostedService
{
    private Task? _task;
    private bool _running = true;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _task = Task.Factory.StartNew(async () =>
        {
            while (_running && !cancellationToken.IsCancellationRequested)
            {
                var result = await gpioController.WaitForEventAsync(
                    pinId, PinEventTypes.Falling & PinEventTypes.Rising,
                    cancellationToken);
                switch (result.EventTypes)
                {
                    case PinEventTypes.Rising:
                        logger.LogInformation("Detected rising edge");
                        await routingService.Route(new GpioRisingEdgeEvent(pinId));
                        break;
                    case PinEventTypes.Falling:
                        logger.LogInformation("Detected falling edge");
                        await routingService.Route(new GpioFallingEdgeEvent(pinId));
                        break;
                }
            }
        }, cancellationToken);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _running = false;
        return _task!;
    }
}