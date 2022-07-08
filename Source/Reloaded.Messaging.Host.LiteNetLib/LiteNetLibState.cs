using LiteNetLib;

namespace Reloaded.Messaging.Host.LiteNetLib;

/// <summary>
/// Encapsulates the state of LiteNetLib at a given point in time.
/// </summary>
public struct LiteNetLibState
{
    /// <summary>
    /// The peer from which the message was received.
    /// </summary>
    public NetPeer Peer { get; }

    /// <summary>
    /// The reader for reading the raw data.
    /// Here for completeness, you probably don't need it.
    /// </summary>
    public NetPacketReader Reader { get; }

    /// <summary>
    /// The channel using which the message was delivered.
    /// </summary>
    public byte Channel { get; }

    /// <summary>
    /// The method with which the message was delivered.
    /// </summary>
    public DeliveryMethod DeliveryMethod { get; }

    /// <summary>
    /// Encapsulates the state of LiteNetLib.
    /// </summary>
    /// <param name="peer">The peer from which the message was received.</param>
    /// <param name="reader">The reader for reading the raw data. You probably don't need it.</param>
    /// <param name="channel">The channel using which the message was delivered.</param>
    /// <param name="deliveryMethod"></param>

    public LiteNetLibState(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
    {
        Peer = peer;
        Reader = reader;
        Channel = channel;
        DeliveryMethod = deliveryMethod;
    }
}