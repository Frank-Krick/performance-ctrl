namespace Franks.Reich.Performance.Ctrl.Configuration;

public class GpioMappingEntry
{
    public int PinId { get; set; }
    public string HandlerType { get; set; } = "";
    public object[] Parameters { get; set; } = [];
}