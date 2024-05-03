using Franks.Reich.Performance.Ctrl.Services.GpioEventRoutingService.Model;
using Franks.Reich.Performance.Ctrl.Services.OpenSoundControlService;
using Microsoft.Extensions.Logging;

namespace Franks.Reich.Performance.Ctrl.Services.GpioEventHandlerRegistryService.GpioEventHandlers;

public class SelectChannelEventHandler(
    IOpenSoundControlService openSoundControlService,
    ILogger<SelectChannelEventHandler> logger) : IGpioEventHandler
{
    public Task Handle(IGpioEvent gpioEvent, object[] parameters)
    {
        logger.LogInformation("Handling gpio event {EventType}, {PinId}",
            gpioEvent.ToString(), gpioEvent.PinId);
        
        var combinedParameters = gpioEvent switch
        {
            GpioRisingEdgeEvent => parameters.Concat([new[] { 1 }]).ToArray(),
            GpioFallingEdgeEvent => parameters.Concat([new[] { 0 }]).ToArray()
        };
        return openSoundControlService.Send("/channels/active", combinedParameters);
    }
}