using System.Runtime;
using Bogus;

namespace Reloaded.Messaging.Benchmarks.Utilities;

public static class FakerExtensions
{
    public static Random GlobalRandom = Random.Shared;

    public static T[] GenerateArray<T>(this Faker<T> faker, int num) where T : class
    {
        var items = GC.AllocateUninitializedArray<T>(num);
        for (int x = 0; x < items.Length; x++)
            items[x] = faker.Generate();
        
        return items;
    }

    public static T[] ArrayElementsFast<T>(this Randomizer random, T[] values, int num) where T : class
    {
        var elements = new T[num];

        for (int x = 0; x < num; x++)
            elements[x] = values[GlobalRandom.Next(0, num)];

        return elements;
    }
}