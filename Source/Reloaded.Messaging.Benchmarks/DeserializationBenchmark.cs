using System.Runtime;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using FastSerialization;
using Microsoft.IO;
using Reloaded.Messaging.Benchmarks.Structures;
using Reloaded.Messaging.Benchmarks.Utilities;
using Reloaded.Messaging.Extras.Runtime;
using Reloaded.Messaging.Interfaces;
using Reloaded.Messaging.Serializer.MessagePack;

namespace Reloaded.Messaging.Benchmarks;

[MemoryDiagnoser]
public class DeserializationBenchmark
{
    public const int UnrollFactor = 5;

    [Params(Constants.DefaultOperationCount)]
    public int NumItems { get; set; }

    private RecyclableMemoryStreamManager _streamManager = new();
    private RecyclableMemoryStream _stream;
    private byte[][] _jsons = null!;
    private byte[][] _msgPacks = null!;

    private SourceGeneratedSystemTextJsonSerializer<ModConfig> _srcGenSystemTextJsonSerializer;
    private SystemTextJsonSerializer<ModConfig> _systemTextJsonSerializer = new();
    private MessagePackSerializer<ModConfig> _messagePackSerializer = new();

    [GlobalSetup]
    public void Setup()
    {
        _srcGenSystemTextJsonSerializer = new(ModConfigContext.Default.ModConfig);

        // Prepare the data by serializing first.
        using (var serializeStream = (RecyclableMemoryStream)_streamManager.GetStream())
        {
            var items = ModConfig.Create<ModConfig>(NumItems);
            _jsons = new byte[NumItems][];
            _msgPacks = new byte[NumItems][];

            for (int x = 0; x < items.Length; x++)
            {
#pragma warning disable CS0618
                serializeStream.SetLength(0);
                _srcGenSystemTextJsonSerializer.Serialize(ref items.GetWithoutBoundsChecks(x), serializeStream);
                _jsons[x] = serializeStream.ToArray();

                serializeStream.SetLength(0);
                _messagePackSerializer.Serialize(ref items.GetWithoutBoundsChecks(x), serializeStream);
                _msgPacks[x] = serializeStream.ToArray();
#pragma warning restore CS0618
            }
        }
        
        _stream = (RecyclableMemoryStream)_streamManager.GetStream();
        GC.Collect();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _jsons = null!;
        _msgPacks = null!;
        GC.Collect();
    }

    [IterationCleanup]
    public void IterationSetup()
    {
        _stream.Seek(0, SeekOrigin.Begin);
    }
    
    [Benchmark]
    public void SystemTextJson() => BenchmarkCommon(_jsons, _systemTextJsonSerializer);

    [Benchmark]
    public void SystemTextJsonSrcGen() => BenchmarkCommon(_jsons, _srcGenSystemTextJsonSerializer);

    [Benchmark]
    public void MessagePack() => BenchmarkCommon(_msgPacks, _messagePackSerializer);

    private void BenchmarkCommon<TSerializer>(byte[][] items, TSerializer serializer) where TSerializer : ISerializer<ModConfig>
    {
        var numIterations = _jsons.Length / UnrollFactor;
        for (int x = 0; x < numIterations; x++)
        {
            var baseIndex = x * UnrollFactor;
            serializer.Deserialize(items.GetWithoutBoundsChecks(baseIndex + 0).AsSpanFast());
            serializer.Deserialize(items.GetWithoutBoundsChecks(baseIndex + 1).AsSpanFast());
            serializer.Deserialize(items.GetWithoutBoundsChecks(baseIndex + 2).AsSpanFast());
            serializer.Deserialize(items.GetWithoutBoundsChecks(baseIndex + 3).AsSpanFast());
            serializer.Deserialize(items.GetWithoutBoundsChecks(baseIndex + 4).AsSpanFast());
        }
    }
}