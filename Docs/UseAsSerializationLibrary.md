## Usage (As Serialization Library)

### Define Serializable Struct/Class

In order to enable serialization for your class, you should implement the `ISerializable` interface.

```csharp
public struct Vector3 : ISerializable
{
	// ISerializable
    public ISerializer GetSerializer() => ReloadedMemorySerializer(false);
    public ICompressor GetCompressor() => null;

	// Members
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
}
```

Returning an `ISerializer` is required. Returning an `ICompressor` is optional.

### Serialize

To serialize an instance, call the extension method `Serializable.Serialize()` in `Reloaded.Messaging.Interfaces.Serializable`.

```csharp
var vector = new Vector3(0, 25, 100);
byte[] data = vector.Serialize();
```

### Deserialize

To serialize an instance, call the extension method `Serializable.Deserialize()` in `Reloaded.Messaging.Interfaces.Serializable`.

```csharp
byte[] data = vector.Serialize();
var newVector = Serializable.Deserialize<Vector3>(data);
```

### Custom Compressors & Serializers
See [ImplementingCompressorsSerializers.md](./ImplementingCompressorsSerializers.md)

### Other Notes

- It is possible to use serializer specific attributes/markup etc. for your struct members.
- Some serializers may require the use of properties for serialization/deserialization.