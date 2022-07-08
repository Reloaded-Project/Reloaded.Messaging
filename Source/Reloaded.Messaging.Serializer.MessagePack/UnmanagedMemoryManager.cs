using System;
using System.Buffers;

namespace Reloaded.Messaging.Serializer.MessagePack;

/// <summary>
/// A MemoryManager over a raw pointer, used for conversion to <see cref="Memory{T}"/> for the serializers that require it.  
/// Pointers passed to this method are expected to be externally pinned.  
/// </summary>
public sealed unsafe class UnmanagedMemoryManager<T> : MemoryManager<T> where T : unmanaged
{
    private T* _pointer;
    private int _length;

    /// <summary>
    /// Creates a UnmanagedMemoryManager from pointer and size.
    /// </summary>
    public UnmanagedMemoryManager(T* pointer, int length)
    {
        _pointer = pointer;
        _length = length;
    }

    /// <summary>
    /// Updates the values behind the current instance.
    /// </summary>
    public void Update(T* pointer, int length)
    {
        _pointer = pointer;
        _length = length;
    }

    /// <summary>
    /// Obtains a span that represents the region.
    /// </summary>
    public override Span<T> GetSpan() => new(_pointer, _length);

    /// <summary>
    /// Returns pointer representing the data [no pin occurs]. 
    /// </summary>
    public override MemoryHandle Pin(int elementIndex = 0) => new(_pointer + elementIndex);

    /// <summary>
    /// Has no effect.
    /// </summary>
    public override void Unpin() { }

    /// <summary>
    /// Releases all resources associated with this object
    /// </summary>
    protected override void Dispose(bool disposing) { }
}