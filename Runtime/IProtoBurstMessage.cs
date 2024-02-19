using ProtoBurst.Packages.ProtoBurst.Runtime;
using Unity.Collections;

namespace ProtoBurst
{
    public interface IProtoBurstMessage
    {
        public int ComputeSize();

        public void WriteTo(ref BufferWriter bufferWriter);

        public SampleTypeUrl GetTypeUrl(Allocator allocator);
    }
}