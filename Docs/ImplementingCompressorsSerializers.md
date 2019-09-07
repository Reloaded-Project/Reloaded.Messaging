## Implementing Compressors & Serializers
Reloaded.Messaging allows you to specify your own serializers and compressors responsible for the serialization of the message to be transmitted.

Adding support for these is very trivial.

### Serializers

To implement a serializer, simply make a class that implements the `ISerializer` interface.

```csharp
public interface ISerializer
{
    TStruct Deserialize<TStruct>(byte[] serialized);
    byte[] Serialize<TStruct>(ref TStruct item);
}
```

A simple example implementation using [MessagePack-CSharp](https://github.com/neuecc/MessagePack-CSharp) could look like this:

```csharp
public class MsgPackSerializer : ISerializer
{
	public TStruct Deserialize<TStruct>(byte[] serialized) => MessagePackSerializer.Deserialize<TStruct>(serialized);

	public byte[] Serialize<TStruct>(ref TStruct item) => MessagePackSerializer.Serialize(item);
}
```

There is no default serializer, however one must be specified.
All serializers can be found in the `Reloaded.Messaging.Serializer` namespace, with serializers available as their own NuGet packages.

### Compressors

Implementing Compressors is virtually identical to implementing a Serializer.

```csharp
public interface ICompressor
{
    byte[] Compress(byte[] data);
    byte[] Decompress(byte[] data);
}
```

A simple example using [ZstdNet](https://github.com/skbkontur/ZstdNet) could look like this:

```csharp
public class ZStandardCompressor : ICompressor, IDisposable
{
    public readonly ZstdNet.Compressor Compressor;
    public readonly Decompressor Decompressor;

    public ZStandardCompressor(CompressionOptions compressionOptions = null, DecompressionOptions decompressionOptions = null)
    {
        Compressor = compressionOptions != null ? new ZstdNet.Compressor(compressionOptions) : new ZstdNet.Compressor();

        Decompressor = decompressionOptions != null ? new Decompressor(decompressionOptions) : new Decompressor();
    }

	// Disposal
    ~ZStandardCompressor()
    {
        Dispose();
    }

    public void Dispose()
    {
        Compressor?.Dispose();
        Decompressor?.Dispose();
        GC.SuppressFinalize(this);
    }

	// ICompressor
    public byte[] Compress(byte[] data) => Compressor.Wrap(data);
    public byte[] Decompress(byte[] data) => Decompressor.Unwrap(data);
}
```

All compressors can be found in the `Reloaded.Messaging.Compressor` namespace, with compressors available as separate NuGet packages.

If `null` is specified for the compressor, no compression will be performed.

