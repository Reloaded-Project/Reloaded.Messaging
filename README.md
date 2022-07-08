<div align="center">
	<h1>Reloaded II: Networking Module</h1>
	<img src="https://i.imgur.com/BjPn7rU.png" width="150" align="center" />
	<br/> <br/>
	<strong><i>Assert.Equal(funnyMessage, Dio)</i></strong>
	<br/> <br/>
</div>

# Introduction

Reloaded.Networking is library that adds support for simple, high performance message packing to existing networking libraries.  

Specifically, it provides a minimal framework for performing the following tasks:  

- Asynchronous message processing for external networking libraries.  
- Sending/Receiving messages with (de)serialization and [optional] (de)compression.  
- Automatically dispatching messages to appropriate handlers (per message type).  

It was originally created for Reloaded II, however has been extended in the hope of becoming a more general purpose library.  
  
This library is heavily optimized for achieving high throughput for messages `< 128KB`.  

## Characteristics
- High performance. (Memory pooling, allocation free).  
- Low networking overhead.  
- Custom serializer/compressor per class type.  
- Simple message packing/protocol.  
- 1 byte overhead for uncompressed, 5 bytes for compressed.  
- Unsafe.  

## Documentation

More information can be found in the [dedicated documentation site](https://reloaded-project.github.io/Reloaded.Messaging).  