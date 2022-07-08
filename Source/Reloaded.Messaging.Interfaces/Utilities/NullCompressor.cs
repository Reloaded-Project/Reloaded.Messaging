using System;

namespace Reloaded.Messaging.Interfaces.Utilities;

/// <summary>
/// Dummy compressor that performs no compression.
/// Use me when specifying TCompressor and return null in structures.
/// </summary>
public class NullCompressor : ICompressor
{
    /// <inheritdoc />
    public int GetMaxCompressedSize(int inputSize)
    {
        return inputSize;
    }

    /// <inheritdoc />
    public int Compress(Span<byte> uncompressedData, Span<byte> compressedData)
    {
        uncompressedData.CopyTo(compressedData);
        return uncompressedData.Length;
    }

    /// <inheritdoc />
    public void Decompress(Span<byte> compressedBuf, Span<byte> uncompressedBuf)
    {
        compressedBuf.CopyTo(uncompressedBuf);
    }
}