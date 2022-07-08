using System;

namespace Reloaded.Messaging.Interfaces;

/// <summary>
/// Encapsulates the minimal interface required by a message dispatcher.
/// </summary>
/// <typeparam name="TExtraData">The extra data </typeparam>
public interface IMessageDispatcher<TExtraData>
{
    /// <summary>
    /// Gets a handler that handles a specific message type.
    /// </summary>
    /// <param name="messageType">The type of message requested.</param>
    /// <returns>Handler for the specific message. Might be null.</returns>
    ref IMessageHandlerBase<TExtraData>? GetHandlerForType(byte messageType);

    /// <summary>
    /// Sets a handler for a specific message type.
    /// </summary>
    void AddOrOverrideHandler(IMessageHandlerBase<TExtraData> handler);

    /// <summary>
    /// Removes a handler assigned to a specific message type.
    /// </summary>
    void RemoveHandler(byte messageType);

    /// <summary>
    /// Given a raw network message, decodes the message and delegates it to an appropriate handling method.
    /// </summary>
    /// <param name="data">Data containing a packed Reloaded.Messaging message.</param>
    /// <param name="extraData">The extra data associated with this request.</param>
    void Dispatch(Span<byte> data, ref TExtraData extraData);
}
