using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace ProtoBurst
{
    public static class NativeListExtensions
    {
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InsertNoResize<T>(this NativeList<T> list, int index, T value) where T : unmanaged
        {
            list.CheckNoResizeHasEnoughCapacity(list.Length + 1);
            list.InsertRange(index, 1);
            list[index] = value;
        }
        
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddRangeNoResize<T>(this NativeList<T> list, NativeArray<T> values) where T : unmanaged
        {
            list.CheckNoResizeHasEnoughCapacity(list.Length + values.Length);

            unsafe
            {
                list.AddRangeNoResize(values.GetUnsafePtr(), values.Length);
            }
        }
        
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddReplicateNoResize<T>(this NativeList<T> list, T value, int count) where T : unmanaged
        {
            list.CheckNoResizeHasEnoughCapacity(list.Length + count);
            list.AddReplicate(value, count);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CheckNoResizeHasEnoughCapacity<T>(this NativeList<T> list, int requestedLength) where T : unmanaged
        {
            if (requestedLength > list.Capacity)
            {
                throw new InvalidOperationException(
                    $"NoResize operations assumes that list capacity is sufficient (Capacity {list.Capacity}, Length {list.Length}), requested length {requestedLength}!");
            }
        }
    }
}