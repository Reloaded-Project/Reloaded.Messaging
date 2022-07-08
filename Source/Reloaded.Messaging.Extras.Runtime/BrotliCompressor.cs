using System;
using System.IO;
using System.IO.Compression;
using Reloaded.Messaging.Interfaces;

namespace Reloaded.Messaging.Extras.Runtime;

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
/// <summary>
/// Provides brotli compression support.
/// </summary>
public struct BrotliCompressor : ICompressor
{
    private byte _quality;
    private byte _window;

    /// <summary>
    /// Creates the default brotli compressor.
    /// </summary>
    public BrotliCompressor()
    {
        _window = 22;
        _quality = 9;
    }

    /// <summary/>
    /// <param name="quality">Quality of encoder. Between 0 and 11. Recommend 9 for size/speed ratio.</param>
    /// <param name="window">Size of window.</param>
    public BrotliCompressor(byte quality, byte window = 22)
    {
        _quality = quality;
        _window = window;
    }

    /// <inheritdoc />
    public int GetMaxCompressedSize(int inputSize) => BrotliEncoder.GetMaxCompressedLength(inputSize);

    /// <inheritdoc />
    public int Compress(Span<byte> uncompressedData, Span<byte> compressedData)
    {
        using var encoder = new BrotliEncoder(_quality, _window);
        encoder.Compress(uncompressedData, compressedData, out _, out var bytesWritten, true);
        return bytesWritten;
    }

    /// <inheritdoc />
    public void Decompress(Span<byte> compressedBuf, Span<byte> uncompressedBuf)
    {
        using var decoder = new BrotliDecoder();
        decoder.Decompress(compressedBuf, uncompressedBuf, out _, out _);
    }
}
#endif