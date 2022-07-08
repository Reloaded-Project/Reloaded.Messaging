# Creating Structures & Messages

## Creating Message Structures

!!! note

    The library heavily (ab)uses generics for optimal code generation and maximum throughput.  

First, a struct or class must be annotated by implementing the `IMessage` interface.  

```csharp
// Structures can specify their own custom serializer(s) and compressor(s).  
// IMessage<TStruct, TSerializer, TCompressor>
public struct Vector3 : IMessage<Vector3, SystemTextJsonSerializer<Vector3>, DummyCompressor>
{
    // IMessage
    public sbyte GetMessageType() => (sbyte)MessageType.Vector3;
    public SystemTextJsonSerializer<Vector3SystemTextJson> GetSerializer() => new();
    public DummyCompressor? GetCompressor() => null;

    // Example Data
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
}
```

Each message must define the following methods:  
- `GetMessageType`: Returns the unique message type. Valid values are between 0 and 127.  
- `GetSerializer`: Returns an instance of the serializer used to serialize/deserialize the message.  
- `GetCompressor`: Returns an instance of the compressor used to compress/decompress the message.  

The type of serializer (`TSerializer`) and type of compressor (`TCompressor`) are defined in the `IMessage` interface; here they are `SystemTextJsonSerializer` and `DummyCompressor` specifically.  

Return `null` for compressor if no compression is requested.  

## Pack Messages

To pack an instance (including serialization & compression), call the extension method `Serialize()` in `Reloaded.Messaging.Messages.MessageWriterExtensions`.  

```csharp
var sample = new Vector3(0.0f, 1.0f, 2.0f);
using var serialized = sample.Serialize(ref sample);

// Access message via `serialized.Span`.
// You can now e.g. send this message over the network.
Client.FirstPeer.Send(serialized.Span, DeliveryMethod.ReliableOrdered);
```

!!! danger

    Serialization result must be disposed before serializing another instance due to internal pooling.  
    The return value `ReusableSingletonMemoryStream` can have at most 1 instance per thread.  

Alternative lower level API: `MessageWriter<TStruct, TSerializer, TCompressor>`.  

## Unpack Messages

!!! info

    Provided for completeness, this is usually automated for you.  
    Only low level API provided.  

```csharp
// Read message header.
HeaderReader.ReadHeader(message.Span, out var messageType, out var compressedSize, out var headerSize);

// Create deserializer & deserialize
// Generic arguments to MessageReader are same as ones to IMessage.
var deserialize  = new MessageReader<Vector3ReloadedMemoryDummyCompression, UnmanagedReloadedMemorySerializer<Vector3ReloadedMemoryDummyCompression>, NullCompressor>(in structure);
Vector3 deserialized = deserialize.Deserialize(message.Span.Slice(headerSize), compressedSize);
```  