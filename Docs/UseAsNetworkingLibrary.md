# Table of Contents
- [Usage (As Networking Library)](#usage-as-networking-library)
  - [How It Works (Summary)](#how-it-works-summary)
  - [A: Unique Message Identifier](#a-unique-message-identifier)
  - [B: Implementing IMessage\<MessageType>](#b-implementing-imessagemessagetype)
  - [C. Connecting Host & Client](#c-connecting-host--client)
  - [D. Receiving Messages](#d-receiving-messages)
  - [E. Send Messages](#e-send-messages)
- [Custom Compressors & Serializers](#custom-compressors--serializers)
- [Other Information](#other-information)
  - [Overriding Serializers & Compressors at Runtime](#overriding-serializers--compressors-at-runtime)
  - [Default Settings (LiteNetLib)](#default-settings-litenetlib)
    - [Connection Requests](#connection-requests)
    - [Packet Handling](#packet-handling)

## Usage (As Networking Library)

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

A serializer must be specified. Compressor is optional.

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

## Custom Compressors & Serializers
See [ImplementingCompressorsSerializers.md](./ImplementingCompressorsSerializers.md)

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