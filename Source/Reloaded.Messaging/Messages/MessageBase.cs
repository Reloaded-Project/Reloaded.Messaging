namespace Reloaded.Messaging.Messages
{
    public unsafe class MessageBase<TMessageType> where TMessageType : unmanaged
    {
        public static TMessageType GetMessageType(byte[] serializedBytes)
        {
            fixed (byte* arrayPtr = serializedBytes)
            {
                return *(TMessageType*) arrayPtr;
            }
        }
    }
}
