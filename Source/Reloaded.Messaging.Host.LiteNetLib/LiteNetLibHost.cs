using System;
using System.Runtime.CompilerServices;
using LiteNetLib;
using LiteNetLib.Utils;
using Reloaded.Messaging.Interfaces;
#if NET5_0_OR_GREATER
using System.Runtime.InteropServices;
#endif

namespace Reloaded.Messaging.Host.LiteNetLib;

/// <summary>
/// Provides a simple client or host based off of LiteNetLib.
/// </summary>
public class LiteNetLibHost<TDispatcher> : IDisposable, IHost<TDispatcher, LiteNetLibState> 
    where TDispatcher : IMessageDispatcher<LiteNetLibState>
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
    public event EventBasedNetListener.OnConnectionRequest? ConnectionRequestEvent;

    /// <summary>
    /// Dispatcher for individual messages sent to the client.
    /// </summary>
    public ref TDispatcher Dispatcher => ref _dispatcher;

    /// <summary>
    /// Exposes the listener with which the manager was created.
    /// </summary>
    public EventBasedNetListener Listener { get; private set; }
    
    /// <summary>
    /// The LiteNetLib manager.
    /// </summary>
    public NetManager Manager { get; private set; }

    /// <summary>
    /// Provides access to first connected peer, useful if client.
    /// </summary>
    public NetPeer? FirstPeer => Manager.FirstPeer;

    private TDispatcher _dispatcher;

    /// <summary>
    /// Provides a simple client or host based off of LiteNetLib.
    /// </summary>
    /// <param name="dispatcher">The dispatcher used to send events to your callback handlers.</param>
    /// <param name="acceptClients">Set to true to accept incoming clients, else reject all requests.</param>
    /// <param name="password">The password necessary to join.</param>
    public LiteNetLibHost(bool acceptClients, TDispatcher dispatcher, string password = "")
    {
        _dispatcher = dispatcher;
        Password = password;
        AcceptClients = acceptClients;
        Listener = new EventBasedNetListener();
        Listener.NetworkReceiveEvent += OnNetworkReceive;
        Listener.ConnectionRequestEvent += ListenerOnConnectionRequestEvent;

        Manager = new NetManager(Listener);
        Manager.UnsyncedEvents = true;
        Manager.AutoRecycle = true;
    }

    /// <inheritdoc />
    ~LiteNetLibHost() => Dispose();

    /// <inheritdoc />
    public void Dispose()
    {
        Manager.Stop();
        GC.SuppressFinalize(this);
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


    /// <inheritdoc />
    public void SendFirstPeer(Span<byte> data)
    {
        FirstPeer.Send(data, DeliveryMethod.ReliableOrdered);
    }

    /// <inheritdoc />
    public void SendToAll(Span<byte> data)
    {
        // TODO: This could be better optimised by sending a PR with Span overload in SendToAll
        var peers = Manager.ConnectedPeerList;
        for (int x = 0; x < peers.Count; x++)
            peers[x].Send(data, DeliveryMethod.ReliableOrdered);
    }

    // On each message received. 
#if NET5_0_OR_GREATER
    [SkipLocalsInit]
#endif
    private void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
    {
        var data = AsSpanFast(reader.RawData, reader.Position, reader.AvailableBytes);
        var state = new LiteNetLibState(peer, reader, channel, deliveryMethod);
        Dispatcher.Dispatch(data, ref state);
    }

    /// <summary>
    /// Provides zero overhead unsafe array to span conversion.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<byte> AsSpanFast(byte[] data, int start, int length)
    {
#if NET5_0_OR_GREATER
        return MemoryMarshal.CreateSpan(ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(data), start), length);
#else
        return data.AsSpan(start, length);
#endif
    }
}