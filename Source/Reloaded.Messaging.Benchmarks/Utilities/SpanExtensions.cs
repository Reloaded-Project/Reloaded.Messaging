using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Reloaded.Messaging.Benchmarks.Utilities;

/// <summary>
/// Extension methods related to spans.
/// </summary>
public static class SpanExtensions
{
    /// <summary>
    /// Provides zero overhead unsafe array to span conversion.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpanFast<T>(this T[] data)
    {
        return MemoryMarshal.CreateSpan(ref MemoryMarshal.GetArrayDataReference(data), data.Length);
    }

    /// <summary>
    /// Provides zero overhead unsafe array to span conversion.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpanFast<T>(this T[] data, int length)
    {
        return MemoryMarshal.CreateSpan(ref MemoryMarshal.GetArrayDataReference(data), length);
    }
}