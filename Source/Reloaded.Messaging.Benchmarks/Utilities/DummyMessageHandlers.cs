using Reloaded.Messaging.Benchmarks.Structures;
using Reloaded.Messaging.Interfaces;

namespace Reloaded.Messaging.Benchmarks.Utilities;

public class DummyMessageHandlerNoDeserialize<TExtraData, TStruct, TSerializer, TCompressor> : IMessageHandlerBase<TExtraData>
    where TStruct : IMessage<TStruct, TSerializer, TCompressor>
    where TCompressor : ICompressor
    where TSerializer : ISerializer<TStruct>
{
    public sbyte GetMessageType() => 0;
    public void HandleMessage(Span<byte> data, int decompressedSize, ref TExtraData extraData)
    {
        // No code
    }
}

public struct DummyCallback : IMsgRefAction<ModConfigMessage, int>
{
    public void OnMessageReceive(ref ModConfigMessage received, ref int data) { }
}