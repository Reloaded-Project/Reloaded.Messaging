using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Reloaded.Messaging.Benchmarks.Utilities;

public static class ArrayExtensions
{

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T GetWithoutBoundsChecks<T>(this T[] items, int index)
    {
        return ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(items), index);
    }
}