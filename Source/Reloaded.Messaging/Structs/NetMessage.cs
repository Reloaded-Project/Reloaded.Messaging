using LiteNetLib;

namespace Reloaded.Messaging.Structs
{
    /// <summary>
    /// Encapsulates a message with a specific structure received from the network as a raw array of bytes.
    /// </summary>
    public struct NetMessage<TStruct>
    {
        /// <summary>
        /// The message received from the peer.
        /// </summary>
        public TStruct Message { get; private set; }

        /// <summary>
        /// The peer which has sent you the message.
        /// </summary>
        public NetPeer Peer { get; private set; }

        /// <summary>
        /// Can be used to read the message, if desired.
        /// </summary>
        public NetPacketReader PacketReader { get; private set; }

        /// <summary>
        /// The method via which the package was delivered.
        /// </summary>
        public DeliveryMethod DeliveryMethod { get; private set; }

        /// <summary>
        /// Encapsulates a raw message received from the network.
        /// </summary>
        /// <param name="message">The message in question.</param>
        /// <param name="rawMessage">The raw message from which this message should be constructed with.</param>
        public NetMessage(ref TStruct message, ref RawNetMessage rawMessage)
        {
            Message = message;
            Peer = rawMessage.Peer;
            PacketReader = rawMessage.PacketReader;
            DeliveryMethod = rawMessage.DeliveryMethod;
        }
    }
}
