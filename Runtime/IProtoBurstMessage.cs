using Unity.Collections;

namespace ProtoBurst
{
    public interface IProtoBurstMessage
    {
        public void WriteTo(ref NativeList<byte> data);

        public FixedString128Bytes TypeUrl { get; }
    }
}