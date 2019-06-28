namespace Reloaded.Messaging.Serialization
{
    public interface ISerializer
    {
        TStruct Deserialize<TStruct>(byte[] serialized);
        byte[] Serialize<TStruct>(ref TStruct item);
    }
}
