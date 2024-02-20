using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

namespace ProtoBurst.Packages.ProtoBurst.Runtime
{
    [BurstCompile]
    public struct BufferWriter
    {
        private NativeList<byte> _bytes;
        
        public int Length => _bytes.Length;

        public BufferWriter(NativeList<byte> bytes)
        {
            _bytes = bytes;
        }
        
        public void WriteByte(byte val)
        {
            _bytes.AddNoResize(val);
        }

        public unsafe void WriteBytes(byte* bytes, int length)
        {
            _bytes.AddRangeNoResize(bytes, length);
        }

        public void WriteLittleEndian32(uint value)
        {
            if (!BitConverter.IsLittleEndian)
                value = ReverseByteOrder(value);

            unsafe
            {
                _bytes.AddRangeNoResize(&value, 4);
            }
        }

        public void WriteLittleEndian64(ulong value)
        {
            if (!BitConverter.IsLittleEndian)
                value = ReverseByteOrder(value);

            unsafe
            {
                _bytes.AddRangeNoResize(&value, 8);
            }
        }

        private static uint ReverseByteOrder(uint val)
        {
            return math.ror(val & 0xFF000000, 24) | math.ror(val & 0x00FF0000, 8) |
                   math.rol(val & 0x0000FF00, 8) | math.rol(val & 0x000000FF, 24);
        }

        private static ulong ReverseByteOrder(ulong val)
        {
            return math.ror(val & 0xFF00000000000000, 56) | math.ror(val & 0x00FF000000000000, 40) |
                   math.ror(val & 0x0000FF0000000000, 24) | math.ror(val & 0x000000FF00000000, 8) |
                   math.rol(val & 0x00000000FF000000, 8) | math.rol(val & 0x0000000000FF0000, 24) |
                   math.rol(val & 0x000000000000FF00, 40) | math.rol(val & 0x00000000000000FF, 56);
        }
    }
}