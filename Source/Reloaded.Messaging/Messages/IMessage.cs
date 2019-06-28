using Reloaded.Messaging.Compression;
using Reloaded.Messaging.Serialization;

namespace Reloaded.Messaging.Messages
{
    /// <summary>
    /// Common interface shared by individual messages.
    /// </summary>
    public interface IMessage<TMessageType> where TMessageType : unmanaged
    {
        TMessageType GetMessageType();
        ISerializer GetSerializer();
        ICompressor GetCompressor();
    }
}
