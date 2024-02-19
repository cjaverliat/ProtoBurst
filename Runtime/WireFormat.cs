using System;
using System.Runtime.CompilerServices;
using Unity.Burst;

namespace ProtoBurst
{
    public static class WireFormat
    {
        private const int TagTypeBits = 3;
        private const uint TagTypeMask = 7;

        public static WireType GetTagWireType(uint tag) => (WireType)((int)tag & 7);

        public static int GetTagFieldNumber(uint tag) => (int)(tag >> 3);
        
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint MakeTag(int fieldNumber, WireType wireType) =>
            (uint)((WireType)(fieldNumber << 3) | wireType);

        [Flags]
        public enum WireType : uint
        {
            VarInt,
            Fixed64,
            LengthDelimited,
            StartGroup,
            EndGroup,
            Fixed32,
        }
    }
}