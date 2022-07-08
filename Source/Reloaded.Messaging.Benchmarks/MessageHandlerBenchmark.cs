using BenchmarkDotNet.Attributes;
using FastSerialization;
using Microsoft.IO;
using Reloaded.Messaging.Benchmarks.Structures;
using Reloaded.Messaging.Benchmarks.Utilities;
using Reloaded.Messaging.Extras.Runtime;
using Reloaded.Messaging.Interfaces.Utilities;

namespace Reloaded.Messaging.Benchmarks;

[MemoryDiagnoser]
public class MessageHandlerBenchmark
{
    private MessageDispatcher<int> _dispatcher;
    private byte[][] _jsons = null!;

    [Params(Constants.DefaultOperationCount)]
    public int NumItems { get; set; }

    private const int UnrollFactor = 10;

    [GlobalSetup]
    public void Setup()
    {
        // Prepare the data by serializing first.
        var streamManager = new RecyclableMemoryStreamManager();
        using (var serializeStream = (RecyclableMemoryStream)streamManager.GetStream())
        {
            var serializer = new SourceGeneratedSystemTextJsonSerializer<ModConfigMessage>(ModConfigMessageContext.Default.ModConfigMessage);
            var items = ModConfig.Create<ModConfigMessage>(NumItems);
            _jsons = new byte[NumItems][];

            for (int x = 0; x < items.Length; x++)
            {
#pragma warning disable CS0618
                serializeStream.SetLength(0);
                serializer.Serialize(ref items.GetWithoutBoundsChecks(x), serializeStream);
                _jsons[x] = serializeStream.ToArray();
#pragma warning restore CS0618
            }
        }

        _dispatcher = new MessageDispatcher<int>();
        _dispatcher.AddOrOverrideHandler(new DummyMessageHandlerNoDeserialize<int, ModConfigMessage, SourceGeneratedSystemTextJsonSerializer<ModConfigMessage>, NullCompressor>());
        GC.Collect();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _jsons = null;
        GC.Collect();
    }


    [Benchmark(Baseline = true)]
    public void HandleMessage()
    {
        // Add handler.
        var dispatcher    = _dispatcher;
        var numIterations = _jsons.Length / UnrollFactor;
        var extraData = 0;

        for (int x = 0; x < numIterations; x++)
        {
            var baseIndex = x * UnrollFactor;
            dispatcher.Dispatch(_jsons.GetWithoutBoundsChecks(baseIndex + 0).AsSpanFast(), ref extraData);
            dispatcher.Dispatch(_jsons.GetWithoutBoundsChecks(baseIndex + 1).AsSpanFast(), ref extraData);
            dispatcher.Dispatch(_jsons.GetWithoutBoundsChecks(baseIndex + 2).AsSpanFast(), ref extraData);
            dispatcher.Dispatch(_jsons.GetWithoutBoundsChecks(baseIndex + 3).AsSpanFast(), ref extraData);
            dispatcher.Dispatch(_jsons.GetWithoutBoundsChecks(baseIndex + 4).AsSpanFast(), ref extraData);
            dispatcher.Dispatch(_jsons.GetWithoutBoundsChecks(baseIndex + 5).AsSpanFast(), ref extraData);
            dispatcher.Dispatch(_jsons.GetWithoutBoundsChecks(baseIndex + 6).AsSpanFast(), ref extraData);
            dispatcher.Dispatch(_jsons.GetWithoutBoundsChecks(baseIndex + 7).AsSpanFast(), ref extraData);
            dispatcher.Dispatch(_jsons.GetWithoutBoundsChecks(baseIndex + 8).AsSpanFast(), ref extraData);
            dispatcher.Dispatch(_jsons.GetWithoutBoundsChecks(baseIndex + 9).AsSpanFast(), ref extraData);
        }
    }
}