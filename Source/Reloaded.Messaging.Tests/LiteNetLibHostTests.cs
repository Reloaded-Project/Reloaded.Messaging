using System;
using System.Net;
using LiteNetLib;
using System.Threading.Tasks;
using Reloaded.Messaging.Host.LiteNetLib;
using Reloaded.Messaging.Messages;
using Reloaded.Messaging.Tests.Messages;
using Reloaded.Messaging.Utilities;
using Xunit;

namespace Reloaded.Messaging.Tests;

public class TestingHosts : IDisposable
{
    private const string DefaultPassword = "SwagSwagSwag";
    public LiteNetLibHost<MessageDispatcher<LiteNetLibState>> Server;
    public LiteNetLibHost<MessageDispatcher<LiteNetLibState>> Client;

    public TestingHosts()
    {
        Server = new LiteNetLibHost<MessageDispatcher<LiteNetLibState>>(true, new MessageDispatcher<LiteNetLibState>(), DefaultPassword);
        Client = new LiteNetLibHost<MessageDispatcher<LiteNetLibState>>(true, new MessageDispatcher<LiteNetLibState>(), DefaultPassword);

        Server.Manager.Start(IPAddress.Loopback, IPAddress.IPv6Loopback, 0);
        Client.Manager.Start(IPAddress.Loopback, IPAddress.IPv6Loopback, 0);
        Client.Manager.Connect(new IPEndPoint(IPAddress.Loopback, Server.Manager.LocalPort), DefaultPassword);

#if DEBUG
        Server.Manager.DisconnectTimeout = int.MaxValue;
        Client.Manager.DisconnectTimeout = int.MaxValue;
#endif
    }

    public void Dispose()
    {
        Server?.Dispose();
        Client?.Dispose();
    }

    [Fact(Timeout = 5000)]
    public async Task SendAndReceiveMessage()
    {
        // Arrange/Setup Host
        var messageHandler = new LiteNetLibMessageHandler();
        messageHandler.Received.AddToDispatcher(messageHandler, ref Server.Dispatcher);
        
        // Send sample message.
        var sample = new Vector3(0.0f, 1.0f, 2.0f);
        using var serialized = sample.Serialize(ref sample);
        Client.SendFirstPeer(serialized.Span);
        
        // Wait for response.
        while (messageHandler.Received.Equals(default))
            await Task.Delay(16);

        Assert.Equal(sample, messageHandler.Received);
        Assert.NotNull(messageHandler.State.Peer);
        Assert.NotNull(messageHandler.State.Reader);
    }

    public class LiteNetLibMessageHandler : IMsgRefAction<Vector3, LiteNetLibState>
    {
        public Vector3 Received;
        public LiteNetLibState State;

        public void OnMessageReceive(ref Vector3 received, ref LiteNetLibState data)
        {
            Received = received;
            State = data;
        }
    }
}