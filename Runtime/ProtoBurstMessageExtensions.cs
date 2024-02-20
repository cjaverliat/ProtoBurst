using ProtoBurst.Packages.ProtoBurst.Runtime;
using Unity.Collections;

namespace ProtoBurst
{
    public static class ProtoBurstMessageExtensions
    {
        public static NativeList<byte> ToBytes<T>(this T message, Allocator allocator)
            where T : unmanaged, IProtoBurstMessage
        {
            var bytes = new NativeList<byte>(message.ComputeSize(), allocator);
            var bufferWriter = new BufferWriter(bytes);
            message.WriteTo(ref bufferWriter);
            return bytes;
        }
    }
}