using System.Net.Sockets;
using CoreOSC;
using CoreOSC.IO;
using Microsoft.Extensions.Logging;

namespace Franks.Reich.Performance.Ctrl.Services.OpenSoundControlService;

public interface IOpenSoundControlService
{
    Task Send(string address, object[] parameters);
}

public class OpenSoundControlService(
    UdpClient client,
    ILogger<OpenSoundControlService> logger) : IOpenSoundControlService
{
    public async Task Send(string address, object[] parameters)
    {
        logger.LogInformation("Sending OSC message to {Address} with {Parameters}", address, parameters);
        await client.SendMessageAsync(new OscMessage(new Address(address), parameters));
        logger.LogInformation("Sent OSC message to {Address} with {Parameters}", address, parameters);
    }
}