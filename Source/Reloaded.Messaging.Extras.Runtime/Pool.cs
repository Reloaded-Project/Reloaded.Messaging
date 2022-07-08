using System;
using System.IO;
using System.Text.Json;

namespace Reloaded.Messaging.Extras.Runtime;

internal static class Pool
{
    [ThreadStatic]
    private static Utf8JsonWriter? _sharedWriter;

    internal static Utf8JsonWriter JsonWriterPerThread()
    {
        if (_sharedWriter == null)
        {
            _sharedWriter = new Utf8JsonWriter(Stream.Null);
            return _sharedWriter;
        }

        return _sharedWriter;
    }
}