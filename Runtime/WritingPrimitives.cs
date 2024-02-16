using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace ProtoBurst
{
    // TODO: implement Insertion methods for all the Write methods + removed any native allocations
    [BurstCompile]
    public static class WritingPrimitives
    {
        public const int VarInt32MaxSize = 4;
        public const int VarInt64MaxSize = 8;
        public const int Int32MaxSize = VarInt32MaxSize;
        public const int Int64MaxSize = VarInt64MaxSize;
        public const int LengthPrefixMaxSize = VarInt32MaxSize;
        public const int Fixed32Size = 4;
        public const int Fixed64Size = 8;
        public const int UInt32MaxSize = VarInt32MaxSize;
        public const int UInt64MaxSize = VarInt64MaxSize;
        public const int TagSize = 2;
        
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteRawByteNoResize(byte value, ref NativeList<byte> data)
        {
            data.AddNoResize(value);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteRawBytesNoResize(ref NativeArray<byte> bytes, ref NativeList<byte> data)
        {
            data.AddRangeNoResize(bytes);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteRawBytesNoResize(byte* ptr, int length, ref NativeList<byte> data)
        {
            data.AddRangeNoResize(ptr, length);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteRawTagNoResize(byte tag, ref NativeList<byte> data)
        {
            WriteRawByteNoResize(tag, ref data);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteRawLittleEndian32NoResize(uint value, ref NativeList<byte> data)
        {
            WriteRawByteNoResize((byte)value, ref data);
            WriteRawByteNoResize((byte)(value >> 8), ref data);
            WriteRawByteNoResize((byte)(value >> 16), ref data);
            WriteRawByteNoResize((byte)(value >> 24), ref data);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteRawLittleEndian64NoResize(ulong value, ref NativeList<byte> data)
        {
            WriteRawByteNoResize((byte)value, ref data);
            WriteRawByteNoResize((byte)(value >> 8), ref data);
            WriteRawByteNoResize((byte)(value >> 16), ref data);
            WriteRawByteNoResize((byte)(value >> 24), ref data);
            WriteRawByteNoResize((byte)(value >> 32), ref data);
            WriteRawByteNoResize((byte)(value >> 40), ref data);
            WriteRawByteNoResize((byte)(value >> 48), ref data);
            WriteRawByteNoResize((byte)(value >> 56), ref data);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InsertRawVarInt32NoResize(uint value, ref NativeList<byte> data, int index)
        {
            if (value < 128U)
            {
                data.InsertNoResize(index, (byte)value);
            }
            else
            {
                while (true)
                {
                    if (value > (uint)sbyte.MaxValue)
                    {
                        data.InsertNoResize(index++, (byte)(value & sbyte.MaxValue | 128));
                        value >>= 7;
                    }
                    else
                    {
                        data.InsertNoResize(index, (byte)value);
                        return;
                    }
                }
            }
        }


        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteRawVarInt32NoResize(uint value, ref NativeList<byte> data)
        {
            InsertRawVarInt32NoResize(value, ref data, data.Length);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteRawVarInt64NoResize(ulong value, ref NativeList<byte> data)
        {
            if (value < 128UL)
            {
                data.AddNoResize((byte)value);
            }
            else
            {
                while (true)
                {
                    if (value > (uint)sbyte.MaxValue)
                    {
                        data.AddNoResize((byte)(value & (ulong)sbyte.MaxValue | 128UL));
                        value >>= 7;
                    }
                    else
                    {
                        data.AddNoResize((byte)value);
                        return;
                    }
                }
            }
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteTagNoResize(int fieldNumber, WireFormat.WireType wireType, ref NativeList<byte> data)
        {
            WriteRawVarInt32NoResize(WireFormat.MakeTag(fieldNumber, wireType), ref data);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteTagNoResize(uint tag, ref NativeList<byte> data)
        {
            WriteRawVarInt32NoResize(tag, ref data);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteTagNoResize(byte tag, ref NativeList<byte> data)
        {
            WriteRawByteNoResize(tag, ref data);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteBoolNoResize(bool value, ref NativeList<byte> data)
        {
            WriteRawByteNoResize(value ? (byte)1 : (byte)0, ref data);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteDoubleNoResize(double value, ref NativeList<byte> data)
        {
            unsafe
            {
                var val = *(ulong*)&value;
                WriteRawLittleEndian64NoResize(val, ref data);
            }
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteFloatNoResize(float value, ref NativeList<byte> data)
        {
            unsafe
            {
                var val = *(uint*)&value;
                WriteRawLittleEndian32NoResize(val, ref data);
            }
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt32NoResize(uint value, ref NativeList<byte> data)
        {
            WriteRawVarInt32NoResize(value, ref data);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt64NoResize(ulong value, ref NativeList<byte> data)
        {
            WriteRawVarInt64NoResize(value, ref data);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt32NoResize(int value, ref NativeList<byte> data)
        {
            WriteRawVarInt32NoResize((uint)value, ref data);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt64NoResize(long value, ref NativeList<byte> data)
        {
            WriteRawVarInt64NoResize((ulong)value, ref data);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteSFixed32NoResize(int value, ref NativeList<byte> data)
        {
            WriteRawLittleEndian32NoResize((uint)value, ref data);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteSFixed64NoResize(long value, ref NativeList<byte> data)
        {
            WriteRawLittleEndian64NoResize((ulong)value, ref data);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteFixed32NoResize(uint value, ref NativeList<byte> data)
        {
            WriteRawLittleEndian32NoResize(value, ref data);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteFixed64NoResize(ulong value, ref NativeList<byte> data)
        {
            WriteRawLittleEndian64NoResize(value, ref data);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteLengthPrefixedFixedStringNoResize(ref FixedString32Bytes value,
            ref NativeList<byte> data)
        {
            WriteLengthPrefixedFixedStringNoResize<FixedString32Bytes>(ref value, ref data);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteLengthPrefixedFixedStringNoResize(ref FixedString64Bytes value,
            ref NativeList<byte> data)
        {
            WriteLengthPrefixedFixedStringNoResize<FixedString64Bytes>(ref value, ref data);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteLengthPrefixedFixedStringNoResize(ref FixedString128Bytes value,
            ref NativeList<byte> data)
        {
            WriteLengthPrefixedFixedStringNoResize<FixedString128Bytes>(ref value, ref data);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteLengthPrefixedFixedStringNoResize(ref FixedString512Bytes value,
            ref NativeList<byte> data)
        {
            WriteLengthPrefixedFixedStringNoResize<FixedString512Bytes>(ref value, ref data);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteLengthPrefixedFixedStringNoResize(ref FixedString4096Bytes value,
            ref NativeList<byte> data)
        {
            WriteLengthPrefixedFixedStringNoResize<FixedString4096Bytes>(ref value, ref data);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteLengthPrefixedFixedStringNoResize<T>(ref T value, ref NativeList<byte> data)
            where T : unmanaged, IUTF8Bytes, IIndexable<byte>
        {
            WriteLengthNoResize(value.Length, ref data);

            unsafe
            {
                WriteRawBytesNoResize(value.GetUnsafePtr(), value.Length, ref data);
            }
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteLengthNoResize(int length, ref NativeList<byte> data)
        {
            WriteRawVarInt32NoResize((uint)length, ref data);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InsertLengthNoResize(int length, ref NativeList<byte> data, int index)
        {
            InsertRawVarInt32NoResize((uint)length, ref data, index);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteLengthPrefixedMessageNoResize<T>(ref T message, ref NativeList<byte> data)
            where T : IProtoBurstMessage
        {
            var prevLength = data.Length;
            message.WriteToNoResize(ref data);
            var newLength = data.Length;
            var length = newLength - prevLength;
            var index = prevLength;
            InsertLengthNoResize(length, ref data, index);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteLengthPrefixedBytesNoResize(ref NativeArray<byte> bytes, ref NativeList<byte> data)
        {
            WriteLengthNoResize(bytes.Length, ref data);
            WriteRawBytesNoResize(ref bytes, ref data);
        }
    }
}