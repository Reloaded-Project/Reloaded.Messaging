using System;
using System.Buffers;
#if NET5_0_OR_GREATER
using System.Runtime.InteropServices;
#endif

namespace Reloaded.Messaging.Utilities;

/// <summary>
/// Represents a temporary array rental from the runtime's ArrayPool.
/// </summary>
/// <typeparam name="T">Type of element to be rented from the runtime.</typeparam>
public struct ArrayRental<T> : IDisposable
{
    private T[] _data;
    private int _count;

    /// <summary>
    /// Rents an array of bytes from the arraypool.
    /// </summary>
    /// <param name="count">Needed amount of bytes.</param>
    public ArrayRental(int count)
    {
        _data = ArrayPool<T>.Shared.Rent(count);
        _count = count;
    }

    /// <summary>
    /// Exposes the raw underlying array, which will likely
    /// be bigger than the number of elements.
    /// </summary>
    public T[] RawArray => _data;

    /// <summary>
    /// Returns the rented array as a span.
    /// </summary>
    public Span<T> Span => SpanExtensions.AsSpanFast(_data, _count);

    /// <summary>
    /// Exposes the number of elements stored by this structure.
    /// </summary>
    public int Count => _count;

    /// <summary>
    /// Returns a reference to the first element.
    /// </summary>
    public ref T FirstElement => ref GetFirstElement();

    /// <summary>
    /// Returns the array to the pool.
    /// </summary>
    public void Dispose() => ArrayPool<T>.Shared.Return(_data, false);

    /// <summary>
    /// Returns a reference to the first element.
    /// </summary>
    private ref T GetFirstElement()
    {
#if NET5_0
        return ref MemoryMarshal.GetArrayDataReference(_data);
#else
        return ref _data[0];
#endif
    }
}