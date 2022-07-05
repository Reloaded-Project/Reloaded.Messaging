﻿using System.Runtime.InteropServices;
using Reloaded.Messaging.Interfaces;
using Reloaded.Messaging.Messages;
using Reloaded.Messaging.Serializer.MessagePack;

namespace Reloaded.Messaging.Tests.Struct;

public struct StringMessage : IMessage<MessageType>
{
    public MessageType GetMessageType() => MessageType.String;
    public ISerializer GetSerializer()  => new MsgPackSerializer();
    public ICompressor GetCompressor()  => null;

    public string Text { get; set; }

    public StringMessage(string text)
    {
        Text = text;
    }

    /* Auto Generated by R# */
    public bool Equals(StringMessage other)
    {
        return string.Equals(Text, other.Text);
    }

    public override bool Equals(object obj)
    {
        return obj is StringMessage other && Equals(other);
    }

    public override int GetHashCode()
    {
        return (Text != null ? Text.GetHashCode() : 0);
    }
}