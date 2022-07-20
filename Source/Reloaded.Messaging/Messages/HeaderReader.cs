using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Reloaded.Messaging.Messages;

/// <summary>
/// Reads the header of a packed message.
/// </summary>
public struct HeaderReader
{
    /// <summary>
    /// Size of standard header, without compression.
    /// </summary>
    public const int StandardHeaderSize = 1;

    /// <summary>
    /// Size of standard header, with compression.
    /// </summary>
    public const int CompressedHeaderSize = 5;

    /// <summary/>
    public const byte CompressionFlag = 0b10000000;


    /// <summary/>
    public const int HeaderDecompressedDataSize = sizeof(uint);

    /// <summary>
    /// Reads the message header of a packaged message.
    /// </summary>
    /// <param name="data">Span containing the header at the start.</param>
    /// <param name="messageType">The type of message in this header.</param>
    /// <param name="sizeAfterDecompression">The size of the compressed payload. This is -1 if there is no compression.</param>
    /// <param name="headerSize">Size of the header at the start of the span.</param>
#if NET5_0_OR_GREATER
    [SkipLocalsInit]
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReadHeader(Span<byte> data, out sbyte messageType, out int sizeAfterDecompression, out int headerSize)
    {
        messageType = (sbyte)MemoryMarshal.GetReference(data);

        if ((messageType & CompressionFlag) == CompressionFlag)
        {
            sizeAfterDecompression = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref MemoryMarshal.GetReference(data), 1));
            messageType = (sbyte)(messageType ^ CompressionFlag);
            if (!BitConverter.IsLittleEndian) // Evaluated at JIT time.
                sizeAfterDecompression = BinaryPrimitives.ReverseEndianness(sizeAfterDecompression);

            headerSize = CompressedHeaderSize;
            return;
        }

        headerSize = StandardHeaderSize;
        sizeAfterDecompression = -1;
    }
}