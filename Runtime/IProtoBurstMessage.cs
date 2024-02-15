using Unity.Collections;

namespace ProtoBurst
{
    public interface IProtoBurstMessage
    {
        public int ComputeMaxSize();

        public void WriteToNoResize(ref NativeList<byte> data); 
        
        public FixedString128Bytes TypeUrl { get; }
    }
}