using System;
#if NET5_0_OR_GREATER
using System.Runtime.CompilerServices;
#endif
using Microsoft.IO;
using Reloaded.Messaging.Utilities;

namespace Reloaded.Messaging.Messages.Disposables;

/// <summary>
/// Wrapper of a recyclable memory stream exposing the raw data underneath.
/// </summary>
public struct ReusableSingletonMemoryStream : IDisposable
{
    internal RecyclableMemoryStream Stream;

    /// <summary>
    /// Provides access to the underlying span.
    /// </summary>
    public Span<byte> Span => SpanExtensions.AsSpanFast(Stream.GetBuffer(), (int)Stream.Length);

    /// <summary>
    /// Provides access to the underlying buffer. 
    /// The length of buffer is denoted by the out value length.
    /// </summary>
    /// <param name="length">Length of the buffer.</param>
#if NET5_0_OR_GREATER
    [SkipLocalsInit]
#endif
    public byte[] GetBuffer(out int length)
    {
        var result = Stream.GetBuffer();
        length = (int)Stream.Length;
        return result;
    }

    /// <summary/>
    /// <param name="stream">The stream in question.</param>
    public ReusableSingletonMemoryStream(RecyclableMemoryStream stream) { Stream = stream; }

    /// <summary>
    /// Disposes the underlying stream.
    /// </summary>
    public void Dispose() => Stream.SetLength(0);
}