using LiteNetLib;

namespace Reloaded.Messaging.Structs
{
    /// <summary>
    /// Encapsulates a message received from the network as a raw array of bytes.
    /// </summary>
    public struct RawNetMessage
    {
        /// <summary>
        /// The contents of the message.
        /// </summary>
        public byte[] Message { get; private set; }

        /// <summary>
        /// The peer from whom the message was received from.
        /// </summary>
        public NetPeer Peer { get; private set; }

        /// <summary>
        /// Used to read the packet internals, if desired.
        /// </summary>
        public NetPacketReader PacketReader { get; private set; }

        /// <summary>
        /// The method via which this message was delivered.
        /// </summary>
        public DeliveryMethod DeliveryMethod { get; private set; }

        /// <summary>
        /// Encapsulates the message received from the network as a raw array of bytes.
        /// </summary>
        /// <param name="message">The message in question.</param>
        /// <param name="peer">The peer from whom the message was received from.</param>
        /// <param name="packetReader">Used to read packet internals, if desired.</param>
        /// <param name="deliveryMethod">The method via which the message was delivered.</param>
        public RawNetMessage(byte[] message, NetPeer peer, NetPacketReader packetReader, DeliveryMethod deliveryMethod)
        {
            Message = message;
            Peer = peer;
            PacketReader = packetReader;
            DeliveryMethod = deliveryMethod;
        }
    }
}
