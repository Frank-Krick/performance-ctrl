using Franks.Reich.Performance.Ctrl.Services.GpioEventHandlerRegistryService;
using Franks.Reich.Performance.Ctrl.Services.GpioEventRoutingService.Model;

namespace Franks.Reich.Performance.Ctrl.Services.GpioEventRoutingService;

public interface IGpioEventRoutingService
{
    Task Route(IGpioEvent gpioEvent);
}

public class GpioEventRouteTarget(
    EventHandlerType handlerType, object[] parameters)
{
    public object[] Parameters { get; } = parameters;
    public EventHandlerType HandlerType { get; } = handlerType;
}

public class GpioEventRoutingService(
    IGpioEventHandlerRegistry registry,
    IReadOnlyDictionary<int, GpioEventRouteTarget> routes) : IGpioEventRoutingService
{
    public Task Route(IGpioEvent gpioEvent)
    {
        if (routes.TryGetValue(gpioEvent.PinId, out var target))
        {
            var handler = registry.Get(target.HandlerType);
            return handler.Handle(gpioEvent, target.Parameters);
        }
        
        return Task.CompletedTask;
    }
}