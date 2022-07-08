using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using Microsoft.IO;
using Reloaded.Messaging.Benchmarks.Structures;
using Reloaded.Messaging.Benchmarks.Utilities;
using Reloaded.Messaging.Extras.Runtime;
using Reloaded.Messaging.Interfaces;
using Reloaded.Messaging.Serializer.MessagePack;

namespace Reloaded.Messaging.Benchmarks;

[MemoryDiagnoser]
public class SerializationBenchmark
{
    public const int UnrollFactor = 5;
    
    [Params(Constants.DefaultOperationCount)]
    public int NumItems { get; set; }

    private ModConfig[]? _items;
    private RecyclableMemoryStreamManager _streamManager = new();
    private RecyclableMemoryStream _stream;

    private SourceGeneratedSystemTextJsonSerializer<ModConfig> _srcGenSystemTextJsonSerializer;
    private SystemTextJsonSerializer<ModConfig> _systemTextJsonSerializer = new();
    private MessagePackSerializer<ModConfig> _messagePackSerializer = new();

    [GlobalSetup]
    public void Setup()
    {
        _srcGenSystemTextJsonSerializer = new(ModConfigContext.Default.ModConfig);

        _items = ModConfig.Create<ModConfig>(NumItems);
        _stream = (RecyclableMemoryStream)_streamManager.GetStream();
        GC.Collect();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _items = null;
        GC.Collect();
    }

    [IterationCleanup]
    public void IterationSetup()
    {
        _stream.Seek(0, SeekOrigin.Begin);
    }
    
    [Benchmark]
    public void SystemTextJson() => BenchmarkCommon(_systemTextJsonSerializer);

    [Benchmark]
    public void SystemTextJsonSrcGen() => BenchmarkCommon(_srcGenSystemTextJsonSerializer);

    [Benchmark]
    public void MessagePack() => BenchmarkCommon(_messagePackSerializer);

    private void BenchmarkCommon<TSerializer>(TSerializer serializer) where TSerializer : ISerializer<ModConfig>
    {
        var numIterations = _items.Length / UnrollFactor;
        for (int x = 0; x < numIterations; x++)
        {
            var baseIndex = x * UnrollFactor;
            serializer.Serialize(ref _items.GetWithoutBoundsChecks(baseIndex + 0), _stream);
            serializer.Serialize(ref _items.GetWithoutBoundsChecks(baseIndex + 1), _stream);
            serializer.Serialize(ref _items.GetWithoutBoundsChecks(baseIndex + 2), _stream);
            serializer.Serialize(ref _items.GetWithoutBoundsChecks(baseIndex + 3), _stream);
            serializer.Serialize(ref _items.GetWithoutBoundsChecks(baseIndex + 4), _stream);
        }
    }
}