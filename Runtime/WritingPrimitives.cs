using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Unity.Burst;
using Unity.Collections;

namespace ProtoBurst
{
    [BurstCompile]
    public static class WritingPrimitives
    {
        private static GCHandle _stringEncodingGCHandle;

        static WritingPrimitives()
        {
            _stringEncodingGCHandle = GCHandle.Alloc(Encoding.UTF8);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteRawByte(byte value, ref NativeList<byte> data)
        {
            data.Add(value);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteRawBytes(ref NativeArray<byte> bytes, ref NativeList<byte> data)
        {
            data.AddRange(bytes);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteRawTag(byte tag, ref NativeList<byte> data)
        {
            WriteRawByte(tag, ref data);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteRawLittleEndian32(uint value, ref NativeList<byte> data)
        {
            var bytes = new NativeArray<byte>(4, Allocator.Temp);
            bytes[0] = (byte)value;
            bytes[1] = (byte)(value >> 8);
            bytes[2] = (byte)(value >> 16);
            bytes[3] = (byte)(value >> 24);
            WriteRawBytes(ref bytes, ref data);
            bytes.Dispose();
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteRawLittleEndian64(ulong value, ref NativeList<byte> data)
        {
            var bytes = new NativeArray<byte>(8, Allocator.Temp);
            bytes[0] = (byte)value;
            bytes[1] = (byte)(value >> 8);
            bytes[2] = (byte)(value >> 16);
            bytes[3] = (byte)(value >> 24);
            bytes[4] = (byte)(value >> 32);
            bytes[5] = (byte)(value >> 40);
            bytes[6] = (byte)(value >> 48);
            bytes[7] = (byte)(value >> 56);
            WriteRawBytes(ref bytes, ref data);
            bytes.Dispose();
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteRawVarInt32(uint value, ref NativeList<byte> data)
        {
            if (value < 128U)
            {
                data.Add((byte)value);
            }
            else
            {
                while (true)
                {
                    if (value > (uint)sbyte.MaxValue)
                    {
                        data.Add((byte)(value & sbyte.MaxValue | 128));
                        value >>= 7;
                    }
                    else
                    {
                        data.Add((byte)value);
                        return;
                    }
                }
            }
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteRawVarInt64(ulong value, ref NativeList<byte> data)
        {
            if (value < 128UL)
            {
                data.Add((byte)value);
            }
            else
            {
                while (true)
                {
                    if (value > (uint)sbyte.MaxValue)
                    {
                        data.Add((byte)(value & (ulong)sbyte.MaxValue | 128UL));
                        value >>= 7;
                    }
                    else
                    {
                        data.Add((byte)value);
                        return;
                    }
                }
            }
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteTag(int fieldNumber, WireFormat.WireType wireType, ref NativeList<byte> data)
        {
            WriteRawVarInt32(WireFormat.MakeTag(fieldNumber, wireType), ref data);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteTag(uint tag, ref NativeList<byte> data)
        {
            WriteRawVarInt32(tag, ref data);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteTag(byte tag, ref NativeList<byte> data)
        {
            WriteRawByte(tag, ref data);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteBool(bool value, ref NativeList<byte> data)
        {
            WriteRawByte(value ? (byte)1 : (byte)0, ref data);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteDouble(double value, ref NativeList<byte> data)
        {
            unsafe
            {
                var val = *(ulong*)&value;
                WriteRawLittleEndian64(val, ref data);
            }
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteFloat(float value, ref NativeList<byte> data)
        {
            unsafe
            {
                var val = *(uint*)&value;
                WriteRawLittleEndian32(val, ref data);
            }
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt32(uint value, ref NativeList<byte> data)
        {
            WriteRawVarInt32(value, ref data);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt64(ulong value, ref NativeList<byte> data)
        {
            WriteRawVarInt64(value, ref data);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt32(int value, ref NativeList<byte> data)
        {
            WriteRawVarInt32((uint)value, ref data);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt64(long value, ref NativeList<byte> data)
        {
            WriteRawVarInt64((ulong)value, ref data);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteSFixed32(int value, ref NativeList<byte> data)
        {
            WriteRawLittleEndian32((uint)value, ref data);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteSFixed64(long value, ref NativeList<byte> data)
        {
            WriteRawLittleEndian64((ulong)value, ref data);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteFixed32(uint value, ref NativeList<byte> data)
        {
            WriteRawLittleEndian32(value, ref data);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteFixed64(ulong value, ref NativeList<byte> data)
        {
            WriteRawLittleEndian64(value, ref data);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteFixedString32Bytes(ref FixedString32Bytes value, ref NativeList<byte> data)
        {
            var bytes = value.AsFixedList().ToNativeArray(Allocator.Temp);
            WriteLength(value.Length, ref data);
            WriteRawBytes(ref bytes, ref data);
            bytes.Dispose();
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteFixedString64Bytes(ref FixedString64Bytes value, ref NativeList<byte> data)
        {
            var bytes = value.AsFixedList().ToNativeArray(Allocator.Temp);
            WriteLength(value.Length, ref data);
            WriteRawBytes(ref bytes, ref data);
            bytes.Dispose();
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteFixedString128Bytes(ref FixedString128Bytes value, ref NativeList<byte> data)
        {
            var bytes = value.AsFixedList().ToNativeArray(Allocator.Temp);
            WriteLength(value.Length, ref data);
            WriteRawBytes(ref bytes, ref data);
            bytes.Dispose();
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteFixedString512Bytes(ref FixedString512Bytes value, ref NativeList<byte> data)
        {
            var bytes = value.AsFixedList().ToNativeArray(Allocator.Temp);
            WriteLength(value.Length, ref data);
            WriteRawBytes(ref bytes, ref data);
            bytes.Dispose();
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteFixedString4096Bytes(ref FixedString4096Bytes value, ref NativeList<byte> data)
        {
            var bytes = value.AsFixedList().ToNativeArray(Allocator.Temp);
            WriteLength(value.Length, ref data);
            WriteRawBytes(ref bytes, ref data);
            bytes.Dispose();
        }

        [BurstDiscard]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteString(string value, ref NativeList<byte> data)
        {
            var encoding = (Encoding)_stringEncodingGCHandle.Target;
            var bytes = new NativeArray<byte>(encoding.GetBytes(value), Allocator.Temp);
            WriteLength(value.Length, ref data);
            WriteRawBytes(ref bytes, ref data);
            bytes.Dispose();
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteLength(int length, ref NativeList<byte> data)
        {
            WriteRawVarInt32((uint)length, ref data);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteMessage<T>(ref T message, ref NativeList<byte> data) where T : IProtoBurstMessage
        {
            var tmp = new NativeList<byte>(Allocator.TempJob);
            message.WriteTo(ref tmp);
            WriteLength(tmp.Length, ref data);
            data.AddRange(tmp.AsArray());
            tmp.Dispose();
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteBytes(ref NativeArray<byte> bytes, ref NativeList<byte> data)
        {
            WriteLength(bytes.Length, ref data);
            WriteRawBytes(ref bytes, ref data);
        }
    }
}