# Serializers & Compressors

## Existing Serializers

All NuGet packages are prefixed with `Reloaded.Messaging` unless otherwise specified.  

| Serializer                                                          | NuGet Package             | Format  | Example Use Case                                                                                  |
|---------------------------------------------------------------------|---------------------------|---------|---------------------------------------------------------------------------------------------------|
| SystemTextJsonSerializer<br>SourceGeneratedSystemTextJsonSerializer | Extras.Runtime            | JSON    | Human Readable Data                                                                               |
| MessagePackSerializer                                               | Serializer.MessagePack    | MsgPack | High Performance, Small Message Size                                                              |
| UnmanagedReloadedMemorySerializer                                   | Serializer.ReloadedMemory | Binary  | Raw struct/byte conversion.  <br>When versioning is not needed and client/host use same endian.   |


## Existing Compressors

| Compressor          | NuGet Package        | Example Use Case                                           |
|---------------------|----------------------|------------------------------------------------------------|
| BrotliCompressor    | Extras.Runtime       | Compressing structured data (e.g. JSON)                    |
| ZStandardCompressor | Compressor.ZStandard | Binary compression. Very good with pre-trained dictionary. |

## Implementing your Own

Implementing serializers and compressors is simple, create structs that implement the `ISerializer` and `ICompressor` interfaces.  
Below are some example(s).  

### Example Serializer

```csharp
/// <inheritdoc />
public struct SystemTextJsonSerializer<TStruct> : ISerializer<TStruct>
{
    /// <summary>
    /// Serialization options.
    /// </summary>
    public JsonSerializerOptions Options { get; private set; }

    /// <summary>
    /// Creates the System.Text.Json based serializer.
    /// </summary>
    public SystemTextJsonSerializer() { Options = new JsonSerializerOptions(); }

    /// <summary>
    /// Creates the System.Text.Json based serializer.
    /// </summary>
    /// <param name="serializerOptions">Options to use for serialization/deserialization.</param>
    public SystemTextJsonSerializer(JsonSerializerOptions serializerOptions) { Options = serializerOptions; }

    /// <inheritdoc />
    public TStruct Deserialize(Span<byte> serialized)
    {
        return JsonSerializer.Deserialize<TStruct>(serialized, Options)!;
    }

    /// <inheritdoc />
    public void Serialize(ref TStruct item, IBufferWriter<byte> writer)
    {
        var write = Pool.JsonWriterPerThread();
        write.Reset(writer);
        JsonSerializer.Serialize(write, item, Options);
    }
}
```

### Example Compressor

```csharp
/// <summary>
/// Provides brotli compression support.
/// </summary>
public struct BrotliCompressor : ICompressor
{
    private byte _quality;
    private byte _window;

    /// <summary>
    /// Creates the default brotli compressor.
    /// </summary>
    public BrotliCompressor()
    {
        _window = 22;
        _quality = 9;
    }

    /// <summary/>
    /// <param name="quality">Quality of encoder. Between 0 and 11. Recommend 9 for size/speed ratio.</param>
    /// <param name="window">Size of window.</param>
    public BrotliCompressor(byte quality, byte window = 22)
    {
        _quality = quality;
        _window = window;
    }

    /// <inheritdoc />
    public int GetMaxCompressedSize(int inputSize) => BrotliEncoder.GetMaxCompressedLength(inputSize);

    /// <inheritdoc />
    public int Compress(Span<byte> uncompressedData, Span<byte> compressedData)
    {
        using var encoder = new BrotliEncoder(_quality, _window);
        encoder.Compress(uncompressedData, compressedData, out _, out var bytesWritten, true);
        return bytesWritten;
    }

    /// <inheritdoc />
    public void Decompress(Span<byte> compressedBuf, Span<byte> uncompressedBuf)
    {
        using var decoder = new BrotliDecoder();
        decoder.Decompress(compressedBuf, uncompressedBuf, out _, out _);
    }
}
```