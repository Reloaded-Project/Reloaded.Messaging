using System;
using System.Runtime.CompilerServices;
using Reloaded.Messaging.Interfaces;
using Reloaded.Messaging.Messages;

namespace Reloaded.Messaging;

/// <summary>
/// The base class for message handlers.
/// </summary>
public class ReferenceMessageHandler<TExtraData, TStruct, TSerializer, TCompressor, TMsgRefAction> : IMessageHandlerBase<TExtraData>
    where TStruct : IMessage<TStruct, TSerializer, TCompressor>
    where TCompressor : ICompressor
    where TSerializer : ISerializer<TStruct>
    where TMsgRefAction : IMsgRefAction<TStruct, TExtraData>
{
    private MessageReader<TStruct, TSerializer, TCompressor> _deserializer;
    private TMsgRefAction _refAction = default!;
    private sbyte _messageType;

    /// <summary>
    /// No default constructor.
    /// </summary>
    private ReferenceMessageHandler() { }

    /// <summary>
    /// Sample structure to extract message deserializer from.
    /// </summary>
    /// <param name="sample">Sample structure</param>
    /// <param name="action">Action to perform for each received message.</param>
    public ReferenceMessageHandler(in TStruct sample, TMsgRefAction action)
    {
        _deserializer = new MessageReader<TStruct, TSerializer, TCompressor>(sample);
        _messageType  = sample.GetMessageType();
        _refAction = action;
    }

    /// <inheritdoc />
    public sbyte GetMessageType() => _messageType;

    /// <inheritdoc />
    public void HandleMessage(Span<byte> data, int decompressedSize, ref TExtraData extraData)
    {
        var deserialized = _deserializer.Deserialize(data, decompressedSize);
        _refAction.OnMessageReceive(ref deserialized, ref extraData);
    }

    /// <summary>
    /// Deserializes the message from a raw array of bytes.
    /// </summary>
    /// <param name="data">The data to deserialize.</param>
    /// <param name="decompressedSize">Decompressed size of data.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TStruct Deserialize(Span<byte> data, int decompressedSize) => _deserializer.Deserialize(data, decompressedSize);
}

/// <summary>
/// Provides a function to execute for the <see cref="ReferenceMessageHandler{TExtraData,TStruct,TSerializer,TCompressor,TMsgRefAction}"/>.
/// </summary>
/// <typeparam name="TStruct">The structure of received message.</typeparam>
/// <typeparam name="TExtraData">Extra data held by the message.</typeparam>
public interface IMsgRefAction<TStruct, TExtraData>
{
    /// <summary>
    /// Runs the code associated with the reference action.
    /// </summary>
    /// <param name="received"></param>
    /// <param name="data"></param>
    void OnMessageReceive(ref TStruct received, ref TExtraData data);
}