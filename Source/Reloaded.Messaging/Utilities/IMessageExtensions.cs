using Reloaded.Messaging.Interfaces;

namespace Reloaded.Messaging.Utilities;

/// <summary>
/// Extensions for classes inheriting from <see cref="IMessage{TStruct,TSerializer,TCompressor}"/>
/// </summary>
public static class MessageExtensions
{
    /// <summary>
    /// Creates a reference message handler from an existing message structure.<br/><br/>
    /// Example usage:<br/>
    /// `sample.CreateMessageHandler(new Vector3BrotliReceiveAction(), 0);`<br/>
    /// `sample.CreateMessageHandler(new Vector3BrotliReceiveAction(), default(TExtraData));`<br/><br/>
    /// Where `sample` inherits from IMessage.
    /// </summary>
    /// <typeparam name="TStruct">Type of structure used.</typeparam>
    /// <typeparam name="TSerializer">Type of serializer used.</typeparam>
    /// <typeparam name="TCompressor">Type of compressor used.</typeparam>
    /// <typeparam name="TExtraData">Extra data, usually received from network interface etc.</typeparam>
    /// <typeparam name="TMsgRefAction">Action to add to the reference message handler.</typeparam>
    /// <param name="structure">The structure containing the message.</param>
    /// <param name="refAction">The action to execute for each message received.</param>
    /// <param name="extraDataSample">Any valid instance of extra data.</param>
    /// <param name="dummy2">Dummy parameter for generic type inference.</param>
    /// <param name="dummy3">Dummy parameter for generic type inference.</param>
    public static ReferenceMessageHandler<TExtraData, TStruct, TSerializer, TCompressor, TMsgRefAction> CreateMessageHandler<TMsgRefAction, TExtraData, TStruct, TSerializer, TCompressor>(this IMessage<TStruct, TSerializer, TCompressor> structure, TMsgRefAction refAction, TExtraData extraDataSample, TSerializer? dummy2 = default, TCompressor? dummy3 = default) 
        where TSerializer : ISerializer<TStruct> 
        where TCompressor : ICompressor
        where TStruct : IMessage<TStruct, TSerializer, TCompressor>
        where TMsgRefAction : IMsgRefAction<TStruct, TExtraData>
    {
        return new ReferenceMessageHandler<TExtraData, TStruct, TSerializer, TCompressor, TMsgRefAction>((TStruct)structure, refAction);
    }

    /// <summary>
    /// Creates a reference message handler from an existing message structure.<br/><br/>
    /// Example usage:<br/>
    /// `sample.AddToDispatcher(new Vector3BrotliReceiveAction(), ref dispatcher);`<br/><br/>
    /// Where `sample` inherits from IMessage.
    /// </summary>
    /// <typeparam name="TStruct">Type of structure used.</typeparam>
    /// <typeparam name="TSerializer">Type of serializer used.</typeparam>
    /// <typeparam name="TCompressor">Type of compressor used.</typeparam>
    /// <typeparam name="TExtraData">Extra data, usually received from network interface etc.</typeparam>
    /// <typeparam name="TMsgRefAction">Action to add to the reference message handler.</typeparam>
    /// <param name="structure">The structure containing the message.</param>
    /// <param name="refAction">The action to execute for each message received.</param>
    /// <param name="dispatcher">The dispatcher to add event handler to.</param>
    /// <param name="dummy2">Dummy parameter for generic type inference.</param>
    /// <param name="dummy3">Dummy parameter for generic type inference.</param>
    public static void AddToDispatcher<TMsgRefAction, TExtraData, TStruct, TSerializer, TCompressor>(this IMessage<TStruct, TSerializer, TCompressor> structure, TMsgRefAction refAction, ref MessageDispatcher<TExtraData> dispatcher, TSerializer? dummy2 = default, TCompressor? dummy3 = default)
        where TSerializer : ISerializer<TStruct>
        where TCompressor : ICompressor
        where TStruct : IMessage<TStruct, TSerializer, TCompressor>
        where TMsgRefAction : IMsgRefAction<TStruct, TExtraData>
    {
        var handler = new ReferenceMessageHandler<TExtraData, TStruct, TSerializer, TCompressor, TMsgRefAction>((TStruct)structure, refAction);
        dispatcher.AddOrOverrideHandler(handler);
    }
}