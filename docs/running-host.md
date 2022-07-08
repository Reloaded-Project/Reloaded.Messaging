# Running a Host

The following article shows how to run an example host.  
For a more complete example, look at Unit Tests.  


## Create a Host

!!! note

    The hosts in `Reloaded.Messaging` do the minimal amount of setup to enable asynchronous message handling.  
    They do not abstract the base libraries; for guidance on using them, please refer to their individual documentation(s).  

```csharp
Server = new LiteNetLibHost<MessageDispatcher<LiteNetLibState>>(true, new MessageDispatcher<LiteNetLibState>());
Client = new LiteNetLibHost<MessageDispatcher<LiteNetLibState>>(true, new MessageDispatcher<LiteNetLibState>());

// Start listening and connect.
// This is LiteNetLib specific code.
Server.Manager.Start(IPAddress.Loopback, IPAddress.IPv6Loopback, 0);
Client.Manager.Start(IPAddress.Loopback, IPAddress.IPv6Loopback, 0);
Client.Manager.Connect(new IPEndPoint(IPAddress.Loopback, Server.Manager.LocalPort), DefaultPassword);
```

All pre-implemented hosts derive from the `IHost<TExtraData>` class.  
`TExtraData` is a generic type used to encapsulate the current state of the host.  
In the case of `LiteNetLib`, it is called `LiteNetLibState`.  

## Sending Messages

Each host implements the functions `SendFirstPeer` and `SendToAll`.  

Recall the serialization example from earlier:  

```csharp
var sample = new Vector3(0.0f, 1.0f, 2.0f);
using var serialized = sample.Serialize(ref sample);

// Client is an IHost.
Client.SendFirstPeer(serialized.Span);
```

## Receiving Messages

In order to receive messages, you must first register a handler for the message type.  
You can do that by calling `AddToDispatcher` on the message type and providing:  
- Class/struct that implements `IMsgRefAction<TMessage, TExtraData>`.  
- The `Dispatcher` from the `IHost` instance.  

```csharp
var messageHandler = new LiteNetLibMessageHandler();
messageHandler.Received.AddToDispatcher(messageHandler, ref Client.Dispatcher);

// Class that will process received `Vector3`s from Host.
public class LiteNetLibMessageHandler : IMsgRefAction<Vector3, LiteNetLibState>
{
    public void OnMessageReceive(ref Vector3 received, ref LiteNetLibState data)
    {
        // Executed on every Vector3 received from host.
        // e.g. You can process message and send response with 
        //      data.Peer.Send();
    }
}
```

Any instance of a class/struct that implements `IMsgRefAction` is fine.  
You can of course implement multiple `IMsgRefAction` in the same class/struct too.  

The hosts are automatically configured to asynchronously receive messages, there is no need to manually call `Receive` or any similar function(s).  