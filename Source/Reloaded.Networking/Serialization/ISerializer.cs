using System;
using System.Collections.Generic;
using System.Text;

namespace Reloaded.Messaging.Serialization
{
    public interface ISerializer
    {
        TStruct Deserialize<TStruct>(byte[] serialized);
        byte[] Serialize<TStruct>(TStruct item);
    }
}
