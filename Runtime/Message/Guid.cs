using Unity.Burst;
using Unity.Collections;

namespace ProtoBurst.Message
{
    [BurstCompile]
    public readonly struct Guid : IProtoBurstMessage
    {
        private static readonly FixedString512Bytes GuidTypeUrl = "fr.liris.plume/fr.liris.Guid";

        private readonly uint _x;
        private readonly uint _y;
        private readonly uint _z;
        private readonly uint _w;

        public Guid(uint x, uint y, uint z, uint w)
        {
            _x = x;
            _y = y;
            _z = z;
            _w = w;
        }
        
        public void WriteTo(ref NativeList<byte> data)
        {
            WritingPrimitives.WriteTag(1, WireFormat.WireType.VarInt, ref data);
            WritingPrimitives.WriteUInt32(_x, ref data);
            WritingPrimitives.WriteTag(2, WireFormat.WireType.VarInt, ref data);
            WritingPrimitives.WriteUInt32(_y, ref data);
            WritingPrimitives.WriteTag(3, WireFormat.WireType.VarInt, ref data);
            WritingPrimitives.WriteUInt32(_z, ref data);
            WritingPrimitives.WriteTag(4, WireFormat.WireType.VarInt, ref data);
            WritingPrimitives.WriteUInt32(_w, ref data);
        }

        public FixedString512Bytes TypeUrl => GuidTypeUrl;
    }
}