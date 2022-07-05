using System;
using LiteNetLib;
using Reloaded.Messaging.Structs;

namespace Reloaded.Messaging;

/// <summary>
/// Provides a simple client or host based off of LiteNetLib.
/// </summary>
public class SimpleHost<TMessageType> : IDisposable where TMessageType : unmanaged
{
    /// <summary>
    /// The password necessary to join this host. If it does not match, incoming clients will be rejected.
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Set to true to accept incoming clients, else reject all clients.
    /// </summary>
    public bool AcceptClients { get; set; }

    /// <summary>
    /// Event for handling connection requests.
    /// </summary>
    public event EventBasedNetListener.OnConnectionRequest ConnectionRequestEvent;

    /// <summary>
    /// Dispatcher for individual <typeparamref name="TMessageType"/>(s) to your events.
    /// </summary>
    public MessageHandler<TMessageType> MessageHandler { get; private set; }

    /// <summary/>
    public EventBasedNetListener Listener { get; private set; }

    /// <summary/>
    public NetManager NetManager { get; private set; }

    /// <summary>
    /// Provides a simple client or host based off of LiteNetLib.
    /// </summary>
    /// <param name="acceptClients">Set to true to accept incoming clients, else reject all requests.</param>
    /// <param name="password">The password necessary to join.</param>
    public SimpleHost(bool acceptClients, string password = "")
    {
        Password = password;
        AcceptClients = acceptClients;
        MessageHandler = new MessageHandler<TMessageType>();
        Listener = new EventBasedNetListener();
        Listener.NetworkReceiveEvent += OnNetworkReceive;
        Listener.ConnectionRequestEvent += ListenerOnConnectionRequestEvent;

        NetManager = new NetManager(Listener);
        NetManager.UnsyncedEvents = true;
        NetManager.AutoRecycle = true;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        NetManager.Stop();
    }

    private void ListenerOnConnectionRequestEvent(ConnectionRequest request)
    {
        if (ConnectionRequestEvent != null)
        {
            ConnectionRequestEvent(request);
            return;
        }

        if (AcceptClients)
            request.AcceptIfKey(Password);
        else
            request.Reject();
    }

    /* On each message received. */
    private void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliverymethod)
    {
        byte[] rawBytes = reader.GetRemainingBytes();
        var rawNetMessage = new RawNetMessage(rawBytes, peer, reader, deliverymethod);
        MessageHandler.Handle(ref rawNetMessage);
    }
}