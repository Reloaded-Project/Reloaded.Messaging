﻿using LiteNetLib;

namespace Reloaded.Messaging.Structs
{
    public struct NetMessage<TStruct>
    {
        public TStruct Message { get; private set; }
        public NetPeer Peer { get; private set; }
        public NetPacketReader PacketReader { get; private set; }
        public DeliveryMethod DeliveryMethod { get; private set; }

        public NetMessage(TStruct message, RawNetMessage rawMessage)
        {
            Message = message;
            Peer = rawMessage.Peer;
            PacketReader = rawMessage.PacketReader;
            DeliveryMethod = rawMessage.DeliveryMethod;
        }
    }
}
