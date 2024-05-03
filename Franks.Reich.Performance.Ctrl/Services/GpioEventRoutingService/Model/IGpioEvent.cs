namespace Franks.Reich.Performance.Ctrl.Services.GpioEventRoutingService.Model;

public interface IGpioEvent
{
    int PinId { get; }
}

public class GpioRisingEdgeEvent(int sourceId) : IGpioEvent
{
    public int PinId { get; } = sourceId;
}

public class GpioFallingEdgeEvent(int sourceId) : IGpioEvent
{
    public int PinId { get; } = sourceId;
}
