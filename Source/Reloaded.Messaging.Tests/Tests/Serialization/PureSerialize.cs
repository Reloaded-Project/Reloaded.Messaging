using System;
using System.Collections.Generic;
using System.Text;
using Reloaded.Messaging.Interfaces;
using Reloaded.Messaging.Tests.Struct;
using Xunit;

namespace Reloaded.Messaging.Tests.Tests.Serialization;

public class PureSerialize
{

    /* Tests Pure Serialization: No Message Passing involved. */ 

    [Fact]
    public void SerializeAndDeserialize()
    {
        var vector = new Vector3(0, 25, 100);
        byte[] data = vector.Serialize();
        var newVector = Serializable.Deserialize<Vector3>(data);
        Assert.Equal(vector, newVector);
    }
}