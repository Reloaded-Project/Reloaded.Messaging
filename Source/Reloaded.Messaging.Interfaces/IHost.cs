using System;
using System.Net;

namespace Reloaded.Messaging.Interfaces;

/// <summary>
/// Encapsulates an individual host.
/// </summary>
public interface IHost<TDispatcher, TExtraData> where TDispatcher : IMessageDispatcher<TExtraData>
{
    /// <summary>
    /// The message dispatcher owned by this host instance.
    /// </summary>
    public ref TDispatcher Dispatcher { get; }
    
    /// <summary>
    /// Sends a message to the first peer (i.e. client to host).
    /// </summary>
    public void SendFirstPeer(Span<byte> data);

    /// <summary>
    /// Sends a message to all connected peers (i.e. host to clients).
    /// </summary>
    public void SendToAll(Span<byte> data);
}