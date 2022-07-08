namespace Reloaded.Messaging.Tests.Messages;

public enum MessageType : byte
{
    Vector3,
    Vector3ReloadedMemoryDummyCompression,
    Vector3MessagePackNoCompression,
    Vector3ZStandard,
    Vector3SystemTextJson,
    Vector3Brotli
}