namespace Reloaded.Messaging.Interfaces;

/// <summary>
/// Defines the minimal interface necessary to bootstrap a 3rd party compressor.
/// </summary>
public interface ICompressor
{
    /// <summary>
    /// Compresses the provided byte array.
    /// </summary>
    /// <param name="data">The data to compress.</param>
    byte[] Compress(byte[] data);

    /// <summary>
    /// Decompresses the provided byte array.
    /// </summary>
    /// <param name="data">The data to decompress.</param>
    byte[] Decompress(byte[] data);
}