using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using Microsoft.IO;
using Reloaded.Messaging.Benchmarks.Structures;
using Reloaded.Messaging.Benchmarks.Utilities;
using Reloaded.Messaging.Interfaces.Utilities;
using Reloaded.Messaging.Messages;

namespace Reloaded.Messaging.Benchmarks;

[MemoryDiagnoser]
public class PackOverheadBenchmark
{
    private ModConfigMessageWithDummySerializer[]? _items;
    private ModConfigMessageWithDummyCompressor[]? _itemsCompressor;

    private RecyclableMemoryStream _stream;
    private RecyclableMemoryStreamManager _streamManager = new();
    private DummySerializer<ModConfigMessageWithDummySerializer> _dummySerializer;

    [Params(Constants.DefaultOperationCount)]
    public int NumItems { get; set; }

    private const int UnrollFactor = 10;

    [GlobalSetup]
    public void Setup()
    {
        _stream = (RecyclableMemoryStream)_streamManager.GetStream();
        _dummySerializer = new();
        _items = ModConfig.Create<ModConfigMessageWithDummySerializer>(NumItems);
        _itemsCompressor = ModConfig.Create<ModConfigMessageWithDummyCompressor>(NumItems);
        GC.Collect();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _items = null;
        GC.Collect();
    }


    [Benchmark(Baseline = true)]
    [SkipLocalsInit]
    public void DummySerializeOnly()
    {
        var numIterations = _items!.Length / UnrollFactor;
        var serializer = _dummySerializer;

        for (int x = 0; x < numIterations; x++)
        {
            var baseIndex = x * UnrollFactor;
            serializer.Serialize(ref _items.GetWithoutBoundsChecks(baseIndex + 0), _stream);
            serializer.Serialize(ref _items.GetWithoutBoundsChecks(baseIndex + 1), _stream);
            serializer.Serialize(ref _items.GetWithoutBoundsChecks(baseIndex + 2), _stream);
            serializer.Serialize(ref _items.GetWithoutBoundsChecks(baseIndex + 3), _stream);
            serializer.Serialize(ref _items.GetWithoutBoundsChecks(baseIndex + 4), _stream);
            serializer.Serialize(ref _items.GetWithoutBoundsChecks(baseIndex + 5), _stream);
            serializer.Serialize(ref _items.GetWithoutBoundsChecks(baseIndex + 6), _stream);
            serializer.Serialize(ref _items.GetWithoutBoundsChecks(baseIndex + 7), _stream);
            serializer.Serialize(ref _items.GetWithoutBoundsChecks(baseIndex + 8), _stream);
            serializer.Serialize(ref _items.GetWithoutBoundsChecks(baseIndex + 9), _stream);
        }
    }

    [Benchmark]
    [SkipLocalsInit]
    public void DummySerialize_And_Pack()
    {
        var numIterations = _items!.Length / UnrollFactor;

        for (int x = 0; x < numIterations; x++)
        {
            var baseIndex = x * UnrollFactor;
            MessageWriter<ModConfigMessageWithDummySerializer, DummySerializer<ModConfigMessageWithDummySerializer>, NullCompressor>.SerializeToRecyclableMemoryStream(ref _items!.GetWithoutBoundsChecks(baseIndex + 0), _stream).Dispose();
            MessageWriter<ModConfigMessageWithDummySerializer, DummySerializer<ModConfigMessageWithDummySerializer>, NullCompressor>.SerializeToRecyclableMemoryStream(ref _items!.GetWithoutBoundsChecks(baseIndex + 1), _stream).Dispose();
            MessageWriter<ModConfigMessageWithDummySerializer, DummySerializer<ModConfigMessageWithDummySerializer>, NullCompressor>.SerializeToRecyclableMemoryStream(ref _items!.GetWithoutBoundsChecks(baseIndex + 2), _stream).Dispose();
            MessageWriter<ModConfigMessageWithDummySerializer, DummySerializer<ModConfigMessageWithDummySerializer>, NullCompressor>.SerializeToRecyclableMemoryStream(ref _items!.GetWithoutBoundsChecks(baseIndex + 3), _stream).Dispose();
            MessageWriter<ModConfigMessageWithDummySerializer, DummySerializer<ModConfigMessageWithDummySerializer>, NullCompressor>.SerializeToRecyclableMemoryStream(ref _items!.GetWithoutBoundsChecks(baseIndex + 4), _stream).Dispose();
            MessageWriter<ModConfigMessageWithDummySerializer, DummySerializer<ModConfigMessageWithDummySerializer>, NullCompressor>.SerializeToRecyclableMemoryStream(ref _items!.GetWithoutBoundsChecks(baseIndex + 5), _stream).Dispose();
            MessageWriter<ModConfigMessageWithDummySerializer, DummySerializer<ModConfigMessageWithDummySerializer>, NullCompressor>.SerializeToRecyclableMemoryStream(ref _items!.GetWithoutBoundsChecks(baseIndex + 6), _stream).Dispose();
            MessageWriter<ModConfigMessageWithDummySerializer, DummySerializer<ModConfigMessageWithDummySerializer>, NullCompressor>.SerializeToRecyclableMemoryStream(ref _items!.GetWithoutBoundsChecks(baseIndex + 7), _stream).Dispose();
            MessageWriter<ModConfigMessageWithDummySerializer, DummySerializer<ModConfigMessageWithDummySerializer>, NullCompressor>.SerializeToRecyclableMemoryStream(ref _items!.GetWithoutBoundsChecks(baseIndex + 8), _stream).Dispose();
            MessageWriter<ModConfigMessageWithDummySerializer, DummySerializer<ModConfigMessageWithDummySerializer>, NullCompressor>.SerializeToRecyclableMemoryStream(ref _items!.GetWithoutBoundsChecks(baseIndex + 9), _stream).Dispose();
        }
    }

    [Benchmark]
    [SkipLocalsInit]
    public void DummySerialize_And_Pack_Compressed()
    {
        // This test is OK because the actual value isn't serialized.
        var numIterations = _itemsCompressor!.Length / UnrollFactor;
        var compressor = new DummyCompressor();

        for (int x = 0; x < numIterations; x++)
        {
            var baseIndex = x * UnrollFactor;
            MessageWriter<ModConfigMessageWithDummyCompressor, DummySerializer<ModConfigMessageWithDummyCompressor>, DummyCompressor>.SerializeToRecyclableMemoryStream(ref _itemsCompressor!.GetWithoutBoundsChecks(baseIndex + 0), _stream, compressor).Dispose();
            MessageWriter<ModConfigMessageWithDummyCompressor, DummySerializer<ModConfigMessageWithDummyCompressor>, DummyCompressor>.SerializeToRecyclableMemoryStream(ref _itemsCompressor!.GetWithoutBoundsChecks(baseIndex + 1), _stream, compressor).Dispose();
            MessageWriter<ModConfigMessageWithDummyCompressor, DummySerializer<ModConfigMessageWithDummyCompressor>, DummyCompressor>.SerializeToRecyclableMemoryStream(ref _itemsCompressor!.GetWithoutBoundsChecks(baseIndex + 2), _stream, compressor).Dispose();
            MessageWriter<ModConfigMessageWithDummyCompressor, DummySerializer<ModConfigMessageWithDummyCompressor>, DummyCompressor>.SerializeToRecyclableMemoryStream(ref _itemsCompressor!.GetWithoutBoundsChecks(baseIndex + 3), _stream, compressor).Dispose();
            MessageWriter<ModConfigMessageWithDummyCompressor, DummySerializer<ModConfigMessageWithDummyCompressor>, DummyCompressor>.SerializeToRecyclableMemoryStream(ref _itemsCompressor!.GetWithoutBoundsChecks(baseIndex + 4), _stream, compressor).Dispose();
            MessageWriter<ModConfigMessageWithDummyCompressor, DummySerializer<ModConfigMessageWithDummyCompressor>, DummyCompressor>.SerializeToRecyclableMemoryStream(ref _itemsCompressor!.GetWithoutBoundsChecks(baseIndex + 5), _stream, compressor).Dispose();
            MessageWriter<ModConfigMessageWithDummyCompressor, DummySerializer<ModConfigMessageWithDummyCompressor>, DummyCompressor>.SerializeToRecyclableMemoryStream(ref _itemsCompressor!.GetWithoutBoundsChecks(baseIndex + 6), _stream, compressor).Dispose();
            MessageWriter<ModConfigMessageWithDummyCompressor, DummySerializer<ModConfigMessageWithDummyCompressor>, DummyCompressor>.SerializeToRecyclableMemoryStream(ref _itemsCompressor!.GetWithoutBoundsChecks(baseIndex + 7), _stream, compressor).Dispose();
            MessageWriter<ModConfigMessageWithDummyCompressor, DummySerializer<ModConfigMessageWithDummyCompressor>, DummyCompressor>.SerializeToRecyclableMemoryStream(ref _itemsCompressor!.GetWithoutBoundsChecks(baseIndex + 8), _stream, compressor).Dispose();
            MessageWriter<ModConfigMessageWithDummyCompressor, DummySerializer<ModConfigMessageWithDummyCompressor>, DummyCompressor>.SerializeToRecyclableMemoryStream(ref _itemsCompressor!.GetWithoutBoundsChecks(baseIndex + 9), _stream, compressor).Dispose();
        }
    }
}