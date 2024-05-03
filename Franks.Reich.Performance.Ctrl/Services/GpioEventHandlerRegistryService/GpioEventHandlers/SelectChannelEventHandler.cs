using Franks.Reich.Performance.Ctrl.Services.GpioEventRoutingService.Model;
using Franks.Reich.Performance.Ctrl.Services.OpenSoundControlService;

namespace Franks.Reich.Performance.Ctrl.Services.GpioEventHandlerRegistryService.GpioEventHandlers;

public class SelectChannelEventHandler(IOpenSoundControlService openSoundControlService) : IGpioEventHandler
{
    public Task Handle(IGpioEvent gpioEvent, object[] parameters) =>
        openSoundControlService.Send("/channel/active", parameters);
}