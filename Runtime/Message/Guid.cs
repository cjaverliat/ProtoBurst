﻿using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;

namespace ProtoBurst.Message
{
    [BurstCompile]
    public readonly struct Guid : IProtoBurstMessage
    {
        private static readonly FixedString128Bytes GuidTypeUrl = "fr.liris.plume/fr.liris.Guid";

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

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ComputeMaxSize()
        {
            return WritingPrimitives.TagSize * 4 + WritingPrimitives.VarInt32MaxSize * 4;
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteToNoResize(ref NativeList<byte> data)
        {
            WritingPrimitives.WriteTagNoResize(1, WireFormat.WireType.VarInt, ref data);
            WritingPrimitives.WriteUInt32NoResize(_x, ref data);
            WritingPrimitives.WriteTagNoResize(2, WireFormat.WireType.VarInt, ref data);
            WritingPrimitives.WriteUInt32NoResize(_y, ref data);
            WritingPrimitives.WriteTagNoResize(3, WireFormat.WireType.VarInt, ref data);
            WritingPrimitives.WriteUInt32NoResize(_z, ref data);
            WritingPrimitives.WriteTagNoResize(4, WireFormat.WireType.VarInt, ref data);
            WritingPrimitives.WriteUInt32NoResize(_w, ref data);
        }

        public FixedString128Bytes TypeUrl => GuidTypeUrl;
    }
}