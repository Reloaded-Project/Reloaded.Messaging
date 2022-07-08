using System;
using System.Runtime.CompilerServices;
#if NET5_0_OR_GREATER
using System.Runtime.InteropServices;
#endif

namespace Reloaded.Messaging.Utilities;

/// <summary>
/// Extension methods related to spans.
/// </summary>
public static class SpanExtensions
{
    /// <summary>
    /// Provides zero overhead unsafe array to span conversion.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpanFast<T>(T[] data, int length)
    {
#if NET5_0_OR_GREATER
        return MemoryMarshal.CreateSpan(ref MemoryMarshal.GetArrayDataReference(data), length);
#else
        return data.AsSpan(0, length);
#endif
    }

    /// <summary>
    /// Converts a pointer to type T into a byte span with equivalent number of bytes.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe Span<byte> AsByteSpanFast<T>(T* data) where T : unmanaged
    {
#if NET5_0_OR_GREATER
        return MemoryMarshal.CreateSpan(ref Unsafe.AsRef<byte>(data), sizeof(T));
#else
        return new Span<byte>(data, sizeof(T));
#endif
    }

    /// <summary>
    /// Slices a span without any bounds checks.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<byte> SliceFast(Span<byte> data, int start)
    {
#if NET5_0_OR_GREATER
        return MemoryMarshal.CreateSpan(ref Unsafe.Add(ref MemoryMarshal.GetReference(data), start), data.Length - start);
#else
        return data.Slice(start);
#endif
    }
}