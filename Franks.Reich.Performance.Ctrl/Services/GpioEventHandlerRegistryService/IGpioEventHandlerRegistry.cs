using Franks.Reich.Performance.Ctrl.Services.GpioEventHandlerRegistryService.GpioEventHandlers;
using Microsoft.Extensions.DependencyInjection;

namespace Franks.Reich.Performance.Ctrl.Services.GpioEventHandlerRegistryService;

public enum EventHandlerType { SelectChannel }

public interface IGpioEventHandlerRegistry
{
    IGpioEventHandler Get(EventHandlerType handlerType);
}

public class GpioEventHandlerRegistry(IServiceProvider services) : IGpioEventHandlerRegistry
{
    public IGpioEventHandler Get(EventHandlerType handlerType) =>
        services.GetRequiredService<SelectChannelEventHandler>();
}