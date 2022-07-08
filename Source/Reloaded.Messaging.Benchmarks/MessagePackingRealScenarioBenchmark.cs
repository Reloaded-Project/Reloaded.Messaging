using BenchmarkDotNet.Attributes;
using Reloaded.Messaging.Benchmarks.Structures;
using Microsoft.IO;
using Reloaded.Messaging.Benchmarks.Utilities;
using Reloaded.Messaging.Interfaces;
using Reloaded.Messaging.Messages;
using Reloaded.Messaging.Utilities;
using Reloaded.Messaging.Extras.Runtime;
using Reloaded.Messaging.Interfaces.Utilities;

namespace Reloaded.Messaging.Benchmarks;

[MemoryDiagnoser]
public class MessagePackingRealScenarioBenchmark
{
    private ModConfigMessage[]? _items;

    private RecyclableMemoryStreamManager _streamManager = new();
    private MessageDispatcher<int> _dispatcher;
    private SourceGeneratedSystemTextJsonSerializer<ModConfigMessage> _srcGenSystemTextJsonSerializer;

    [Params(Constants.DefaultOperationCount)]
    public int NumItems { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _srcGenSystemTextJsonSerializer = new SourceGeneratedSystemTextJsonSerializer<ModConfigMessage>(ModConfigMessageContext.Default.ModConfigMessage);
        _dispatcher = new MessageDispatcher<int>();
        _items = ModConfig.Create<ModConfigMessage>(NumItems);
        GC.Collect();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _items = null;
        GC.Collect();
    }


    [Benchmark(Baseline = true)]
    public void SerializeOnly_NoPack_To_SingleBuffer()
    {
        using var buffer = (RecyclableMemoryStream)_streamManager.GetStream();
        for (int x = 0; x < _items!.Length; x++)
            _srcGenSystemTextJsonSerializer.Serialize(ref _items.GetWithoutBoundsChecks(x), buffer);
    }

    [Benchmark]
    public void SerializeOnly_NoPack_To_BufferPerMessage()
    {
        for (int x = 0; x < _items!.Length; x++)
        {
            using var buffer = (RecyclableMemoryStream)_streamManager.GetStream();
            _srcGenSystemTextJsonSerializer.Serialize(ref _items.GetWithoutBoundsChecks(x), buffer);
        }
    }

    [Benchmark]
    public void Serialize_And_Pack()
    {
        var dummy = new ModConfigMessage();
        for (int x = 0; x < _items!.Length; x++)
        {
            using var serialized = dummy.Serialize(ref _items.GetWithoutBoundsChecks(x));
            // Calling span might lead to a memory copy operation, hence to make the
            // test fair, we need to call it on the baseline too.
            var _ = serialized.Span;
        }
    }

    [Benchmark]
    public void Serialize_And_Pack_And_Handle()
    {
        // Copy should be cheap here, dispatcher is small.
        var dispatcher = _dispatcher;
        dispatcher.AddOrOverrideHandler(new DummyMessageHandlerNoDeserialize<int, ModConfigMessage, SourceGeneratedSystemTextJsonSerializer<ModConfigMessage>, NullCompressor>());
        var dummyMsg = new ModConfigMessage();
        int dummy = 0;

        for (int x = 0; x < _items!.Length; x++)
        {
            using var serialized = dummyMsg.Serialize(ref _items.GetWithoutBoundsChecks(x));
            dispatcher.Dispatch(serialized.Span, ref dummy);
        }

        dispatcher.RemoveHandler(ModConfigMessage.MessageType);
    }

    [Benchmark]
    public void Serialize_And_Pack_And_Handle_And_Unpack_And_Deserialize()
    {
        // Copy should be cheap here, dispatcher is small.
        var dispatcher = _dispatcher;
        _items[0].AddToDispatcher(new DummyCallback(), ref _dispatcher);

        var dummyMsg = new ModConfigMessage();
        int dummy = 0;

        for (int x = 0; x < _items!.Length; x++)
        {
            using var serialized = dummyMsg.Serialize(ref _items.GetWithoutBoundsChecks(x));
            dispatcher.Dispatch(serialized.Span, ref dummy);
        }

        dispatcher.RemoveHandler(ModConfigMessage.MessageType);
    }
}