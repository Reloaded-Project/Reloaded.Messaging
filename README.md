<div align="center">
	<h1>Reloaded II: Networking Module</h1>
	<img src="https://i.imgur.com/BjPn7rU.png" width="150" align="center" />
	<br/> <br/>
	<strong><i>Assert.Equal(funnyMessage, Dio)</i></strong>
	<br/> <br/>
</div>

# Packages
**Reloaded.Messaging:** <a href="https://www.nuget.org/packages/Reloaded.Messaging"><img src="https://img.shields.io/nuget/v/Reloaded.Messaging.svg" alt="NuGet" /></a>  <a href="https://ci.appveyor.com/project/sewer56lol/reloaded-messaging"><img src="https://ci.appveyor.com/api/projects/status/u5f1ucq7d76m376g?svg=true" alt="Build Status" /></a>

**Reloaded.Messaging.Serializer.MessagePack**:  <a href="https://www.nuget.org/packages/Reloaded.Messaging.Serializer.MessagePack"><img src="https://img.shields.io/nuget/v/Reloaded.Messaging.Serializer.MessagePack.svg" alt="NuGet" /></a>

**Reloaded.Messaging.Serializer.ReloadedMemory**: <a href="https://www.nuget.org/packages/Reloaded.Messaging.Serializer.ReloadedMemory"><img src="https://img.shields.io/nuget/v/Reloaded.Messaging.Serializer.ReloadedMemory.svg" alt="NuGet" /></a>

**Reloaded.Messaging.Compressor.ZStandard**: <a href="https://www.nuget.org/packages/Reloaded.Messaging.Compressor.ZStandard"><img src="https://img.shields.io/nuget/v/Reloaded.Messaging.Compressor.ZStandard.svg" alt="NuGet" /></a>

# Introduction
Reloaded.Networking is [Reloaded II](https://github.com/Reloaded-Project/Reloaded-II/)'s extensible "event-like" solution for passing messages across a local or remote network that extends on the base functionality of [LiteNetLib](https://github.com/RevenantX/LiteNetLib) by Ruslan Pyrch (RevenantX) .

It has been slightly extended in the hope of becoming more general purpose, perhaps to be reused in other projects.

## Idea
`Reloaded.Networking` is a simple barebones library to solve a deceptively annoying problem: Writing code that distinguishes the type of message received over a network and performs a specific action.

## Characteristics
- Minimal networking overhead in most use cases (1 byte)*.
- Choice of serializer/compressor on a per type (struct/class) basis.
- Simple to use.

*Assuming user has less than 256 unique types of network messages. 

*Alternative unmanaged types (e.g. short, int) can be specified increasing overhead to `sizeof(type)` and respectively increasing max unique types.*

## Usage

### How It Works (Summary)
A. User specifies or chooses an individual unmanaged type `TMessageType` (recommend enum), where each unique value corresponds to different message structure.
B. User implements interface `IMessage<TMessageType>` in types they want to send over the network.
C. User creates `SimpleHost` instance(s) for Server/Client with type from `A` as generic type.
D. User registers methods to handle different values for `TMessageType`.

And then message sending/receiving can proceed.

Note: A complete working example can be found in the basic test collection, `Reloaded.Messaging.Tests`.

### A: Unique Message Identifier
A unique message identifier `TMessageType` can be any unmanaged type. 
The recommended type is enum.

```csharp
// We have less than 256 values, so use byte.
// Default for enum is int, but we don't need extra 3 bytes of overhead in every message.
public enum MessageType : byte 
{
    String,
    Vector3
}
```

### B: Implementing IMessage\<MessageType>
The interface specifies the compressor and serializer used to pack the specified structure. 
It acts as a contract and therefore should match between the server and client.

No information about the serializer or compressor is sent with any of the packets as that would incur unnecessary additional overhead.

```csharp
public struct Vector3 : IMessage<MessageType>
{
	// IMessage
    public MessageType GetMessageType() => MessageType.Vector3;
    public ISerializer GetSerializer() => new MsgPackSerializer(true);
    public ICompressor GetCompressor() => null;

	// Members
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
}
```

A serializer mist be specified. Compressor is optional.

### C. Connecting Host & Client 

The following example creates a new server and client on the local machine and connects them to each other.

```csharp
// DefaultPassword = "RandomString"
// MessageType = enum (byte) (see above)
SimpleServer = new SimpleHost<MessageType>(true, DefaultPassword);
SimpleClient = new SimpleHost<MessageType>(false, DefaultPassword);

SimpleServer.NetManager.Start(IPAddress.Loopback, IPAddress.IPv6Loopback, 0);
SimpleClient.NetManager.Start(IPAddress.Loopback, IPAddress.IPv6Loopback, 0);
SimpleClient.NetManager.Connect(new IPEndPoint(IPAddress.Loopback, SimpleServer.NetManager.LocalPort), DefaultPassword);
```

### D. Receiving Messages
```csharp
// Register a function "Handler" to deal with incoming messages of type Vector3. (MessageType is obtained from IMessage Interface)
SimpleClient.MessageHandler.AddOrOverrideHandler<Vector3>(Handler);

static void Handler(ref NetMessage<Vector3> netMessage)
{
	var vector3 = netMessage.Message;
    // Do something with Vector3
}
```

### E. Send Messages
To send a message, create an instance of  `Message<TMessageType, TStruct>`, where `TStruct` is a struct from `B` that inherits `IMessage<TMessageType>`.

```csharp
var vectorMessage = new Message<MessageType, Vector3>(message); // Wraps the message for sending.
byte[] data = vectorMessage.Serialize(); // Serializes and compresses using Serializer/Compressor defined in IMessage implementation.

SimpleServer.NetManager.FirstPeer.Send(data, DeliveryMethod.ReliableOrdered); // Regular LiteNetLib usage.
```

## Extensibility
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

## Other Information

### Overriding Serializers & Compressors at Runtime
*Note: This feature is mainly intended for benchmarking and testing.*
*Changes are client-side (program-side) only and not broadcasted to clients etc.*

It is possible to override the compressor and/or serializer used to handle a specific type at runtime.

Usage Examples:
```csharp
// Override Vector3
Overrides.SerializerOverride[typeof(Vector3)] = new MsgPackSerializer(false);
Overrides.CompressorOverride[typeof(Vector3)] = null;

// Remove overrides for Vector3
Overrides.SerializerOverride.Remove(typeof(Vector3));
Overrides.CompressorOverride.Remove(typeof(Vector3));
```

### Default Settings (LiteNetLib)
`SimpleHost` uses the following default settings for the LiteNetLib library.

#### Connection Requests
- Subscribes to `ConnectionRequestEvent`, allowing clients to connect only with password set in constructor. (Which can be changed after instantiation)

#### Packet Handling
- Sets `UnsyncedEvents` to true. Messages are received automatically on background thread.
- Sets `AutoRecycle` to true. Automatically recycling NetPacketReader.
- Subscribes to `NetworkReceiveEvent`, to automatically handle incoming packets that have been assigned to the `MessageHandler`.
