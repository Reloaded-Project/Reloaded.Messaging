using Bogus;
using Reloaded.Messaging.Extras.Runtime;
using Reloaded.Messaging.Interfaces;
using Reloaded.Messaging.Interfaces.Utilities;
using Reloaded.Messaging.Serializer.MessagePack;
using System.Text.Json.Serialization;
using Reloaded.Messaging.Benchmarks.Utilities;

namespace Reloaded.Messaging.Benchmarks.Structures;

/// <summary>
/// Copy of Reloaded-II's ModConfig
/// </summary>
public class ModConfig
{

    /* Class members. */
    public string? ModId { get; set; }
    public string? ModName { get; set; }
    public string? ModAuthor { get; set; }
    public string? ModVersion { get; set; }
    public string? ModDescription { get; set; }
    public string? ModDll { get; set; }
    
    public string? ModIcon { get; set; }
    public string? ModR2RManagedDll32 { get; set; }
    public string? ModR2RManagedDll64 { get; set; }
    public string? ModNativeDll32 { get; set; }
    public string? ModNativeDll64 { get; set; }
    public bool? IsLibrary { get; set; }
    public string? ReleaseMetadataFileName { get; set; }
    
    public Dictionary<string, object>? PluginData { get; set; }

    public bool? IsUniversalMod { get; set; }

    public string[]? ModDependencies { get; set; }
    public string[]? OptionalDependencies { get; set; }
    public string[]? SupportedAppId { get; set; }

    public static T[] Create<T>(int numConfigs) where T : ModConfig, new()
    {
        var dependencies = new Faker<string>().CustomInstantiator(f => f.Hacker.Random.Hash()).GenerateArray(numConfigs);
        var applications = new Faker<string>().CustomInstantiator(f => f.System.FileName(".exe")).GenerateArray(Math.Max(numConfigs / 100, 1));

        var faker = new Faker<T>().CustomInstantiator(f => 
        {
            var x = new T();
            x.ModId = f.Hacker.Random.Hash();
            x.ModName = f.Name.FullName();
            x.ModAuthor = f.Internet.UserName();
            x.ModVersion = f.System.Version().ToString();
            x.ModDescription = f.Lorem.Sentences(3);
            x.ModDll = f.System.FileName(".dll");
            x.ModIcon = f.System.CommonFileName(".png");
            x.ModR2RManagedDll32 = f.System.FileName(".dll").OrNull(f, 0.8f);
            x.ModR2RManagedDll64 = f.System.FileName(".dll").OrNull(f, 0.8f);
            x.ModNativeDll32 = f.System.FileName(".dll").OrNull(f, 0.8f);
            x.ModNativeDll64 = f.System.FileName(".dll").OrNull(f, 0.8f);
            x.IsLibrary = f.Random.Bool(0.01f);
            x.ReleaseMetadataFileName = f.System.FileName(".json");
            x.IsLibrary = f.Random.Bool(0.01f);
            x.ModDependencies = f.Random.ArrayElementsFast(dependencies, f.Random.Int(0, 5));
            x.OptionalDependencies = f.Random.ArrayElementsFast(dependencies, f.Random.Int(0, 1));
            x.SupportedAppId = f.Random.ArrayElementsFast(applications, 1);
            return x;
        });

        return faker.GenerateArray(numConfigs);
    }
}

public class ModConfigMessage : ModConfig, IMessage<ModConfigMessage, SourceGeneratedSystemTextJsonSerializer<ModConfigMessage>, NullCompressor>
{
    public const int MessageType = 0;

    public SourceGeneratedSystemTextJsonSerializer<ModConfigMessage> GetSerializer() => new (ModConfigMessageContext.Default.ModConfigMessage);
    public NullCompressor GetCompressor() => null;
    public sbyte GetMessageType() => MessageType;
}

public class ModConfigMessageWithDummySerializer : ModConfig, IMessage<ModConfigMessageWithDummySerializer, DummySerializer<ModConfigMessageWithDummySerializer>, NullCompressor>
{
    public const int MessageType = 0;

    public DummySerializer<ModConfigMessageWithDummySerializer> GetSerializer() => new();
    public NullCompressor GetCompressor() => null;
    public sbyte GetMessageType() => MessageType;
}

public class ModConfigMessageWithDummyCompressor: ModConfig, IMessage<ModConfigMessageWithDummyCompressor, DummySerializer<ModConfigMessageWithDummyCompressor>, DummyCompressor>
{
    public const int MessageType = 0;

    public DummySerializer<ModConfigMessageWithDummyCompressor> GetSerializer() => new();
    public DummyCompressor GetCompressor() => new();
    public sbyte GetMessageType() => MessageType;
}

[JsonSerializable(typeof(ModConfigMessage), GenerationMode = JsonSourceGenerationMode.Default)]
internal partial class ModConfigMessageContext : JsonSerializerContext { }

[JsonSerializable(typeof(ModConfig), GenerationMode = JsonSourceGenerationMode.Default)]
internal partial class ModConfigContext : JsonSerializerContext { }