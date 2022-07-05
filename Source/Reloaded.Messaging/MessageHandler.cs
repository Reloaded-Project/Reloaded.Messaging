using System.Collections.Generic;
using Reloaded.Messaging.Interfaces;
using Reloaded.Messaging.Messages;
using Reloaded.Messaging.Structs;

namespace Reloaded.Messaging
{
    /// <summary>
    /// Provides a generic mechanism for dispatching messages received from a client or server.
    /// Works by assigning functions to specified message "types", declared by TMessageType.
    /// </summary>
    /// <typeparam name="TMessageType">Type of value to map to individual message handlers.</typeparam>
    public class MessageHandler<TMessageType> where TMessageType : unmanaged
    {
        private Dictionary<TMessageType, RawNetMessageHandler> _mapping;

        /// <summary/>
        public MessageHandler()
        {
            _mapping = new Dictionary<TMessageType, RawNetMessageHandler>();
        }

        /// <summary>
        /// Given a raw network message, decodes the message and delegates it to an appropriate handling method.
        /// </summary>
        public void Handle(ref RawNetMessage parameters)
        {
            var messageType = MessageBase<TMessageType>.GetMessageType(parameters.Message);
            if (_mapping.TryGetValue(messageType, out RawNetMessageHandler value))
            {
                value(ref parameters);
            }
        }

        /// <summary>
        /// Sets a method to execute handling a specific <typeparamref name="TMessageType"/>
        /// </summary>
        public void AddOrOverrideHandler<TStruct>(Handler<TStruct> handler) where TStruct : IMessage<TMessageType>, new()
        {
            var messageType = MessageExtensions.GetMessageType(new TStruct());
            AddOrOverrideHandler(messageType, handler);
        }

        /// <summary>
        /// Sets a method to execute handling a specific <typeparamref name="TMessageType"/>
        /// </summary>
        public void AddOrOverrideHandler<TStruct>(TMessageType messageType, Handler<TStruct> handler) where TStruct : IMessage<TMessageType>, new()
        {
            RawNetMessageHandler parameters = delegate (ref RawNetMessage rawMessage)
            {
                var message = Message<TMessageType, TStruct>.Deserialize(rawMessage.Message);
                var netMessage = new NetMessage<TStruct>(ref message, ref rawMessage);
                handler(ref netMessage);
            };

            _mapping[messageType] = parameters;
        }

        /// <summary>
        /// Removes the current method assigned to a handle a message of a specific <typeparamref name="TMessageType"/>
        /// </summary>
        public void RemoveHandler(TMessageType messageType)
        {
            _mapping.Remove(messageType);
        }

        /// <summary/>
        public delegate void Handler<TStruct>(ref NetMessage<TStruct> netMessage);
        private delegate void RawNetMessageHandler(ref RawNetMessage rawNetMessage);
    }
}
