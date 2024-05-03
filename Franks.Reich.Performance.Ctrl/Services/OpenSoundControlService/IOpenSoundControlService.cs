using System.Net.Sockets;
using CoreOSC;
using CoreOSC.IO;

namespace Franks.Reich.Performance.Ctrl.Services.OpenSoundControlService;

public interface IOpenSoundControlService
{
    Task Send(string address, object[] parameters);
}

public class OpenSoundControlService(UdpClient client) : IOpenSoundControlService
{
    public Task Send(string address, object[] parameters) =>
        client.SendMessageAsync(new OscMessage(new Address(address), parameters));
}