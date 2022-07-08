# Message Structure

Each message uses the following structure:  

```
- [7 bits] Message Type
- [1 bit ] Compression Flag
- [0/4 bytes] Decompressed Size
- [Remaining bytes] Message
```

Messages are deliberately simple, to avoid unnecessary complexity.  

## Message Type
This is denoted by `TMessageType` within the library, and is user defined.  
Each message type can be assigned 1 handler.  

Type: Signed byte  
Values: `0x0 - 0x7F`  
Mask: `0b0111_1111`  

## Compression Flag

Set in leftmost bit of message type.  
1 if the message contains compression, else 0.  

Type: Single bit  
Values: `0x0 - 0x1`  
Mask: `0b1000_0000`  

## Decompressed Size
Expected size after decompression.  
Used for compressors to allow for memory pre-allocation.  

Type: Signed 32-bit integer.  
Optional: Used only if struct defines a compressor.  

Values:  
- `0x0 - 0x7FFFFFFF`: Expected size of decompressed message.  

Uses `Little Endian`.  

The resulting data is always compressed, regardless of whether it ended up being larger or smaller than the original in order to improve throughput (avoids a memory copy).  

Reason for signed vs unsigned lies in limitations of some languages (incl. .NET), memory pools often don't support 4GB arrays.  

## Message

Raw message in bytes.  
How message is stored depends on serializer.  