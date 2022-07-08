# Sample Benchmarks

Performed using Version 2.0.0 on the following configuration:  

```
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1806 (21H2)
Intel Core i7-4790K CPU 4.00GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.100-preview.5.22307.18
  [Host] : .NET 6.0.6 (6.0.622.26707), X64 RyuJIT
```

All benchmarks performed on single thread; albeit library is safe to use with multiple threads.

## Message Unpacker & Dispatcher Benchmarks

This is a benchmark of `MessageDispatcher<T>`.  

This benchmark measures the time taken to:  
- Decode Message Header (Unpack)  
- Send Message to Handler for Deserialization  

```
|        Method | NumItems |     Mean |     Error |    StdDev | Ratio | Operations/s | Allocated |
|-------------- |--------- |---------:|----------:|----------:|------:|-------------:|----------:|
| HandleMessage |   100000 | 1.038 ms | 0.0167 ms | 0.0148 ms |  1.00 |  96332329.33 |       2 B |
```

Around 96 million messages per second.  

## Message Creation Benchmarks

This is a benchmark of `MessageWriter.Serialize`.  

This benchmark measures the time taken to:  
- Create & Dispose container used for storing packed message. (`Microsoft.IO.RecyclableMemoryStream`)  
- Create Message Header.  
- Pack Message (combine header + raw data).  

```
|                             Method | NumItems |         Mean |      Error |     StdDev |    Ratio | RatioSD |  Operations/s | Allocated |
|----------------------------------- |--------- |-------------:|-----------:|-----------:|---------:|--------:|--------------:|----------:|
|        DummySerializeOnly_BASELINE |   100000 |     13.85 us |   0.062 us |   0.058 us |     1.00 |    0.00 | 7222495640.56 |         - |
|            DummySerialize_And_Pack |   100000 |  7,294.25 us |  53.658 us |  47.567 us |   526.71 |    4.73 |   13709434.85 |       8 B |
| DummySerialize_And_Pack_Compressed |   100000 | 20,060.07 us | 228.341 us | 213.590 us | 1,448.88 |   17.92 |    4985028.25 |      50 B |
```

Key:  
- DummySerializeOnly_BASELINE: `Overhead of non-packing related code.`  
- DummySerialize_And_Pack: `Time taken to pack all messages without compression.`  
- DummySerialize_And_Pack_Compressed: `Time taken to pack all messages with compression header.`  

## Real World Scenario Benchmarks

The following benchmark measures the time taken to serialize:  
- [A real world configuration file](https://github.com/Reloaded-Project/Reloaded-II/blob/32d5e132391d96814ea983cda231c271c43828e0/source/Reloaded.Mod.Loader.IO/Config/ModConfig.cs#L4) into JSON.  
- And run it through the library.  

|                                                   Method | NumItems |     Mean |   Error |  StdDev | Ratio | RatioSD | Operations/s |      Gen 0 |     Allocated |
|--------------------------------------------------------- |--------- |---------:|--------:|--------:|------:|--------:|-------------:|-----------:|--------------:|
|                     SerializeOnly_NoPack_To_SingleBuffer |   100000 | 155.6 ms | 2.76 ms | 2.59 ms |  1.00 |    0.00 |    642735.51 |          - |   1,614,672 B |
|                 SerializeOnly_NoPack_To_BufferPerMessage |   100000 | 205.6 ms | 2.36 ms | 2.21 ms |  1.32 |    0.02 |    486273.70 |  6000.0000 |  26,400,624 B |
|                                       Serialize_And_Pack |   100000 | 157.6 ms | 1.43 ms | 1.34 ms |  1.01 |    0.02 |    634681.18 |          - |         252 B |
|                            Serialize_And_Pack_And_Handle |   100000 | 156.9 ms | 1.97 ms | 1.84 ms |  1.01 |    0.02 |    637216.58 |          - |         420 B |
| Serialize_And_Pack_And_Handle_And_Unpack_And_Deserialize |   100000 | 581.8 ms | 7.88 ms | 7.37 ms |  3.74 |    0.06 |    171887.71 | 50000.0000 | 209,719,272 B |

- SerializeOnly_NoPack_To_SingleBuffer: `Time taken to serialize all messages into a single memory buffer`.  
- SerializeOnly_NoPack_To_BufferPerMessage: `Time taken to serialize all messages into 1 memory buffer per message`.  
- Serialize_And_Pack: `Time taken to serialize and pack every message using the library.`  
- Serialize_And_Pack_And_Handle: `Time taken to serialize, pack and send message via MessageDispatcher<T>.`  
- Serialize_And_Pack_And_Handle_And_Unpack_And_Deserialize: `Provided for completeness to measure theoretical throughput.`  

From these results we can extrapolate that:  
- Overhead for packing a message is 1-2ms per 100,000 items; (`SerializeOnly_NoPack_To_SingleBuffer` - `Serialize_And_Pack`).  

## Additional Benchmarks

Further benchmarks can be found in the `Reloaded.Messaging.Benchmarks` project.  