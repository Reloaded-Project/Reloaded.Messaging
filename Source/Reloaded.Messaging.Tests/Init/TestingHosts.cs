using System;
using System.Net;

namespace Reloaded.Messaging.Tests.Init;

public class TestingHosts : IDisposable
{
    private const string DefaultPassword = "CutenessIsJustice";
    public SimpleHost<MessageType> SimpleServer;
    public SimpleHost<MessageType> SimpleClient;

    public TestingHosts()
    {
        SimpleServer = new SimpleHost<MessageType>(true, DefaultPassword);
        SimpleClient = new SimpleHost<MessageType>(false, DefaultPassword);

        SimpleServer.NetManager.Start(IPAddress.Loopback, IPAddress.IPv6Loopback, 0);
        SimpleClient.NetManager.Start(IPAddress.Loopback, IPAddress.IPv6Loopback, 0);
        SimpleClient.NetManager.Connect(new IPEndPoint(IPAddress.Loopback, SimpleServer.NetManager.LocalPort), DefaultPassword);

#if DEBUG
        SimpleServer.NetManager.DisconnectTimeout = int.MaxValue;
        SimpleClient.NetManager.DisconnectTimeout = int.MaxValue;
#endif
    }

    public void Dispose()
    {
        SimpleServer?.Dispose();
        SimpleClient?.Dispose();
    }
}