using System.Buffers;
using Reloaded.Messaging.Interfaces;

namespace Reloaded.Messaging.Benchmarks.Utilities;

public struct DummySerializer<TStruct> : ISerializer<TStruct> where TStruct : new()
{
    public TStruct Deserialize(Span<byte> serialized)
    {
        return new TStruct();
    }

    public void Serialize(ref TStruct item, IBufferWriter<byte> writer)
    {
    }
}