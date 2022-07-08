using System;

namespace Reloaded.Messaging.Interfaces;

/// <summary>
/// Defines the minimal interface necessary to bootstrap a 3rd party compressor.
/// </summary>
public interface ICompressor
{
    /// <summary>
    /// Gets the maximum possible size of a compressed file.
    /// </summary>
    /// <param name="inputSize">The input size.</param>
    /// <returns>The maximum possible size after compression.</returns>
    int GetMaxCompressedSize(int inputSize);

    /// <summary>
    /// Compresses the provided byte array.
    /// </summary>
    /// <param name="uncompressedData">The data to compress.</param>
    /// <param name="compressedData">The data to compress.</param>
    /// <returns>Number of compressed bytes.</returns>
    int Compress(Span<byte> uncompressedData, Span<byte> compressedData);

    /// <summary>
    /// Decompresses the provided byte array.
    /// </summary>
    /// <param name="compressedBuf">The data to decompress.</param>
    /// <param name="uncompressedBuf">The buffer containing uncompressed data.</param>
    void Decompress(Span<byte> compressedBuf, Span<byte> uncompressedBuf);
}