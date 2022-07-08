using System;
using Reloaded.Messaging.Interfaces;
using ZstdNet;

namespace Reloaded.Messaging.Compressor.ZStandard;

/// <summary>
/// Creates a new compressor using ZStd.
/// </summary>
public struct ZStandardCompressor : ICompressor, IDisposable
{
    /// <summary>
    /// Instance of the compressor.
    /// </summary>
    public readonly ZstdNet.Compressor Compressor;

    /// <summary>
    /// Instance of the decompressor.
    /// </summary>
    public readonly Decompressor Decompressor;

    /// <summary>
    /// Creates a new compressor based off of ZStandard.
    /// </summary>
    /// <param name="compressionOptions">Sets the options used for compression.</param>
    /// <param name="decompressionOptions">Sets the options used for decompression.</param>
    public ZStandardCompressor(CompressionOptions compressionOptions = null, DecompressionOptions decompressionOptions = null)
    {
        Compressor = compressionOptions != null ? new ZstdNet.Compressor(compressionOptions) : new ZstdNet.Compressor();
        Decompressor = decompressionOptions != null ? new Decompressor(decompressionOptions) : new Decompressor();
    }

    /// <summary>
    /// Creates a new compressor based off of ZStandard.
    /// </summary>
    public ZStandardCompressor()
    {
        Compressor = new ZstdNet.Compressor();
        Decompressor = new Decompressor();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Compressor?.Dispose();
        Decompressor?.Dispose();
    }

    /// <inheritdoc />
    public int GetMaxCompressedSize(int inputSize) => ZstdNet.Compressor.GetCompressBound(inputSize);

    /// <inheritdoc />
    public int Compress(Span<byte> uncompressed, Span<byte> compressed) => Compressor.Wrap(uncompressed, compressed);

    /// <inheritdoc />
    public void Decompress(Span<byte> compressedData, Span<byte> uncompressedData) => Decompressor.Unwrap(compressedData, uncompressedData);
}