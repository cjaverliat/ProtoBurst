using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;

namespace ProtoBurst
{
    [BurstCompile]
    public static class ProtoBurstMessageExtensions
    {
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeList<byte> SerializeLengthPrefixed<T>(this T message, Allocator allocator)
            where T : IProtoBurstMessage
        {
            var bytes = new NativeList<byte>(allocator);
            WritingPrimitives.WriteLengthPrefixedMessage(ref message, ref bytes);
            return bytes;
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeList<byte> Serialize<T>(this T message, Allocator allocator)
            where T : IProtoBurstMessage
        {
            var bytes = new NativeList<byte>(allocator);
            message.WriteTo(ref bytes);
            return bytes;
        }
    }
}