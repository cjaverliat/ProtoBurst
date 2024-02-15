using System;
using Unity.Collections;

namespace ProtoBurst
{
    public static class NativeListExtensions
    {
        public static void InsertNoResize<T>(this NativeList<T> list, int index, T value) where T : unmanaged
        {
            list.EnsureCapacity(list.Length + 1);
            list.InsertRange(index, 1);
            list[index] = value;
        }

        public static void EnsureCapacity<T>(this NativeList<T> list, int requestedLength) where T : unmanaged
        {
            if (requestedLength > list.Capacity)
            {
                throw new InvalidOperationException(
                    $"NoResize operations assumes that list capacity is sufficient (Capacity {list.Capacity}, Length {list.Length}), requested length {requestedLength}!");
            }
        }
    }
}