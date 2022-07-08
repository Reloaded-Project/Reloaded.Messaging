using System;

namespace Reloaded.Messaging.Interfaces;

/// <summary>
/// Common interface shared by types that perform direct deserialization and handling of messages.
/// </summary>
/// <typeparam name="TExtraData">Type of extra data attached to the message. Usually state of network library.</typeparam>
public interface IMessageHandlerBase<TExtraData>
{
    /// <summary>
    /// Returns the unique message type/id for this message.
    /// </summary>
    sbyte GetMessageType();

    /// <summary>
    /// Handles a specific incoming message.
    /// </summary>
    /// <param name="data">The raw data, without message header, ready for decompression and deserialization.</param>
    /// <param name="decompressedSize">Expected size after decompression.</param>
    /// <param name="extraData">Extra data. Usually state of networking library used.</param>
    void HandleMessage(Span<byte> data, int decompressedSize, ref TExtraData extraData);
}