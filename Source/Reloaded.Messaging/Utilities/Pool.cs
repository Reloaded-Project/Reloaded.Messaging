using Microsoft.IO;
using System;

namespace Reloaded.Messaging.Utilities;

internal class Pool
{
    private static RecyclableMemoryStreamManager _recyclableMemoryStreamManager = new();

    static Pool() => _recyclableMemoryStreamManager.AggressiveBufferReturn = true;

    [ThreadStatic]
    private static RecyclableMemoryStream? _compressedMemoryStream;

    [ThreadStatic]
    private static RecyclableMemoryStream? _messageMemoryStream;

    internal static RecyclableMemoryStream CompressionStreamPerThread()
    {
        if (_compressedMemoryStream == null)
        {
            _compressedMemoryStream = (RecyclableMemoryStream)_recyclableMemoryStreamManager.GetStream();
            return _compressedMemoryStream;
        }

        return _compressedMemoryStream;
    }

    internal static RecyclableMemoryStream MessageStreamPerThread()
    {
        if (_messageMemoryStream == null)
        {
            _messageMemoryStream = (RecyclableMemoryStream)_recyclableMemoryStreamManager.GetStream();
            return _messageMemoryStream;
        }

        return _messageMemoryStream;
    }
}