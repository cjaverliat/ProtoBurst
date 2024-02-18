using ProtoBurst.Packages.ProtoBurst.Runtime;
using Unity.Collections;

namespace ProtoBurst
{
    public interface IProtoBurstMessage
    {
        public int ComputeMaxSize();

        public void WriteToNoResize(ref NativeList<byte> data);
        
        public void WriteTo(ref NativeList<byte> data);
        
        public SampleTypeUrl TypeUrl { get; }
    }
}