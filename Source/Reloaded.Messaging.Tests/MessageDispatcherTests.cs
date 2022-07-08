using Reloaded.Messaging.Messages;
using Reloaded.Messaging.Tests.Messages;
using Reloaded.Messaging.Utilities;
using Xunit;

namespace Reloaded.Messaging.Tests;

/// <summary>
/// Provides various tests for the high end message handler.
/// </summary>
public class MessageDispatcherTests
{
    [Fact]
    public void Dispatch_GetHandler_WithNoHandler_ReturnsNull()
    {
        // Arrange
        var dispatcher = new MessageDispatcher<int>();
        
        // Check all slots
        for (int x = 0; x < sbyte.MaxValue; x++)
            Assert.Null(dispatcher.GetHandlerForType((byte)x));
    }

    [Fact]
    public void Dispatch_WithHandler_ReturnNotNull()
    {
        // Arrange
        var dispatcher = new MessageDispatcher<int>();
        var sample = new Vector3(0.5f, 1.0f, 2.0f);

        // Add to dispatcher.
        var receiveAction = new Vector3ReceiveAction();
        sample.AddToDispatcher(receiveAction, ref dispatcher);

        // Pack message.
        Assert.NotNull(dispatcher.GetHandlerForType((byte)sample.GetMessageType()));
    }

    [Fact]
    public void Dispatch_WithRemovedHandler_ReturnNull()
    {
        // Arrange
        var dispatcher = new MessageDispatcher<int>();
        var sample = new Vector3(0.5f, 1.0f, 2.0f);

        // Add to dispatcher.
        var receiveAction = new Vector3ReceiveAction();
        sample.AddToDispatcher(receiveAction, ref dispatcher);

        // Check for presence, then after removal.
        Assert.NotNull(dispatcher.GetHandlerForType((byte)sample.GetMessageType()));
        dispatcher.RemoveHandler((byte)sample.GetMessageType());
        Assert.Null(dispatcher.GetHandlerForType((byte)sample.GetMessageType()));
    }

    [Fact]
    public void Dispatch_WithHandler_ReceivesMessage()
    {
        // Arrange
        var dispatcher = new MessageDispatcher<int>();
        var sample     = new Vector3(0.5f, 1.0f, 2.0f);

        // Add to dispatcher.
        var receiveAction = new Vector3ReceiveAction();
        sample.AddToDispatcher(receiveAction, ref dispatcher);
        
        // Pack message.
        using var serialized = sample.Serialize(ref sample);
        var extraData = 42;
        dispatcher.Dispatch(serialized.Span, ref extraData);

        // Check message was received.
        Assert.Equal(extraData, receiveAction.ExtraData);
        Assert.Equal(sample, receiveAction.Received);
    }

    public class Vector3ReceiveAction : IMsgRefAction<Vector3, int>
    {
        public Vector3 Received;
        public int ExtraData;

        public void OnMessageReceive(ref Vector3 received, ref int data)
        {
            Received  = received;
            ExtraData = data;
        }
    }
}