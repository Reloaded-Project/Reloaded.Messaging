using System;
using Reloaded.Messaging.Interfaces;
using Reloaded.Messaging.Messages;
using Reloaded.Messaging.Utilities;

#if NET5_0_OR_GREATER
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#endif

namespace Reloaded.Messaging;

/// <summary>
/// Provides a generic mechanism for dispatching messages to registered handlers.
/// </summary>
/// <typeparam name="TExtraData">Type of parameter.</typeparam>
public struct MessageDispatcher<TExtraData> : IMessageDispatcher<TExtraData>
{
    // Note: We allocate 255 handlers to remove a branch in `GetHandlerForType` in cases where bad actor may pass invalid value.
    // bit wasteful memory wise but it is what it is.

    private IMessageHandlerBase<TExtraData>?[] _handlers;

    /// <summary/>
    public MessageDispatcher() => _handlers = new IMessageHandlerBase<TExtraData>[byte.MaxValue];

    /// <summary>
    /// Gets a handler that handles a specific message type.
    /// </summary>
    /// <param name="messageType">The type of message requested.</param>
    /// <returns>Handler for the specific message. Might be null.</returns>
    public ref IMessageHandlerBase<TExtraData>? GetHandlerForType(byte messageType)
    {
#if NET5_0_OR_GREATER
        ref var writer   = ref MemoryMarshal.GetArrayDataReference(_handlers);
        return ref Unsafe.Add(ref writer, (int)messageType);
#else
        return ref _handlers[messageType];
#endif
    }

    /// <summary>
    /// Sets a handler for a specific message type.
    /// </summary>
    public void AddOrOverrideHandler(IMessageHandlerBase<TExtraData> handler) => _handlers[handler.GetMessageType()] = handler;

    /// <summary>
    /// Removes a handler assigned to a specific message type.
    /// </summary>
    public void RemoveHandler(byte messageType) => _handlers[messageType] = null;

    /// <summary>
    /// Given a raw network message, decodes the message and delegates it to an appropriate handling method.
    /// </summary>
    /// <param name="data">Data containing a packed Reloaded.Messaging message.</param>
    /// <param name="extraData">The extra data associated with this request.</param>
#if NET5_0_OR_GREATER
    [SkipLocalsInit]    
#endif
    public void Dispatch(Span<byte> data, ref TExtraData extraData)
    {
        HeaderReader.ReadHeader(data, out var messageType, out var sizeAfterCompression, out var headerSize);
        ref var handler = ref GetHandlerForType((byte)messageType);
        handler?.HandleMessage(SpanExtensions.SliceFast(data, headerSize), sizeAfterCompression, ref extraData);
    }
}

