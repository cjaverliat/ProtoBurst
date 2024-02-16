using Unity.Collections;

namespace ProtoBurst
{
    public static class ProtoBurstMessageExtensions
    {
        public static NativeArray<byte> SerializeLengthPrefixed<T>(this T message, Allocator allocator)
            where T : IProtoBurstMessage
        {
            var bytes = new NativeList<byte>(message.ComputeMaxSize() + WritingPrimitives.LengthPrefixMaxSize,
                Allocator.Persistent);
            WritingPrimitives.WriteLengthPrefixedMessageNoResize(ref message, ref bytes);
            var result = bytes.ToArray(allocator);
            bytes.Dispose();
            return result;
        }

        public static NativeArray<byte> Serialize<T>(this T message, Allocator allocator)
            where T : IProtoBurstMessage
        {
            var bytes = new NativeList<byte>(message.ComputeMaxSize(), Allocator.Persistent);
            message.WriteToNoResize(ref bytes);
            var result = bytes.ToArray(allocator);
            bytes.Dispose();
            return result;
        }
    }
}