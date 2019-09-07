<div align="center">
	<h1>Reloaded II: Networking Module</h1>
	<img src="https://i.imgur.com/BjPn7rU.png" width="150" align="center" />
	<br/> <br/>
	<strong><i>Assert.Equal(funnyMessage, Dio)</i></strong>
	<br/> <br/>
</div>

# Packages
**Reloaded.Messaging:** <a href="https://www.nuget.org/packages/Reloaded.Messaging"><img src="https://img.shields.io/nuget/v/Reloaded.Messaging.svg" alt="NuGet" /></a>

**Reloaded.Messaging.Interfaces:** <a href="https://www.nuget.org/packages/Reloaded.Messaging.Interfaces"><img src="https://img.shields.io/nuget/v/Reloaded.Messaging.Interfaces.svg" alt="NuGet" /></a>

**Reloaded.Messaging.Serializer.MessagePack**:  <a href="https://www.nuget.org/packages/Reloaded.Messaging.Serializer.MessagePack"><img src="https://img.shields.io/nuget/v/Reloaded.Messaging.Serializer.MessagePack.svg" alt="NuGet" /></a>

**Reloaded.Messaging.Serializer.ReloadedMemory**: <a href="https://www.nuget.org/packages/Reloaded.Messaging.Serializer.ReloadedMemory"><img src="https://img.shields.io/nuget/v/Reloaded.Messaging.Serializer.ReloadedMemory.svg" alt="NuGet" /></a>

**Reloaded.Messaging.Serializer.SystemTextJson**: <a href="https://www.nuget.org/packages/Reloaded.Messaging.Serializer.SystemTextJson"><img src="https://img.shields.io/nuget/v/Reloaded.Messaging.Serializer.SystemTextJson.svg" alt="NuGet" /></a>

**Reloaded.Messaging.Serializer.NewtonsoftJson**: <a href="https://www.nuget.org/packages/Reloaded.Messaging.Serializer.NewtonsoftJson"><img src="https://img.shields.io/nuget/v/Reloaded.Messaging.Serializer.NewtonsoftJson.svg" alt="NuGet" /></a>

**Reloaded.Messaging.Compressor.ZStandard**: <a href="https://www.nuget.org/packages/Reloaded.Messaging.Compressor.ZStandard"><img src="https://img.shields.io/nuget/v/Reloaded.Messaging.Compressor.ZStandard.svg" alt="NuGet" /></a>

# Introduction
Reloaded.Networking is [Reloaded II](https://github.com/Reloaded-Project/Reloaded-II/)'s Networking and Serialization library. The main goal for the library is to provide an extensible "event-like" solution for passing messages across a local or remote network that extends on the base functionality of [LiteNetLib](https://github.com/RevenantX/LiteNetLib) by Ruslan Pyrch (RevenantX) .

It has been slightly extended in the hope of becoming more general purpose, perhaps to be reused in other projects.

## Idea
`Reloaded.Networking` is a simple barebones library to solve a deceptively annoying problem: Writing code that distinguishes the type of message received over a network and performs a specific action.

## Characteristics
- Minimal networking overhead in most use cases (1 byte)*.
- Choice of serializer/compressor on a per type (struct/class) basis.
- Simple to use.

*Assuming user has less than 256 unique types of network messages. 

*Alternative unmanaged types (e.g. short, int) can be specified increasing overhead to `sizeof(type)` and respectively increasing max unique types.*

## Usage

[Usage: As Networking Library](./Docs/UseAsNetworkingLibrary.md)
[Usage: As Serialization Library]((./Docs/UseAsSerializationLibrary.md)
[Adding 3rd Party Compressors & Serializers](./Docs/ImplementingCompressorsSerializers.md)