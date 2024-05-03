using System.Reflection.Metadata;
using Franks.Reich.Performance.Ctrl.Services.GpioEventRoutingService.Model;

namespace Franks.Reich.Performance.Ctrl.Services.GpioEventHandlerRegistryService;

public interface IGpioEventHandler
{
    Task Handle(IGpioEvent gpioEvent, object[] parameters);
}