using System.Text;
using Google.Protobuf;
using ProtoBurst.Packages.ProtoBurst.Runtime;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace ProtoBurst
{
    // TODO: implement Insertion methods for all the Write methods + removed any native allocations
    [BurstCompile]
    public static class BufferWriterExtensions
    {
        public const int Fixed32Size = 4;
        public const int Fixed64Size = 8;

        public static int ComputeInt32Size(int value)
        {
            return ComputeVarIntSize((uint)value);
        }

        public static int ComputeInt64Size(long value)
        {
            return ComputeVarIntSize((ulong)value);
        }

        public static int ComputeVarIntSize(ulong value)
        {
            if (((long)value & sbyte.MinValue) == 0L)
                return 1;
            if (((long)value & -16384L) == 0L)
                return 2;
            if (((long)value & -2097152L) == 0L)
                return 3;
            if (((long)value & -268435456L) == 0L)
                return 4;
            if (((long)value & -34359738368L) == 0L)
                return 5;
            if (((long)value & -4398046511104L) == 0L)
                return 6;
            if (((long)value & -562949953421312L) == 0L)
                return 7;
            if (((long)value & -72057594037927936L) == 0L)
                return 8;
            return ((long)value & long.MinValue) == 0L ? 9 : 10;
        }

        public static int ComputeTagSize(uint tag)
        {
            return ComputeVarIntSize(tag);
        }

        public static int ComputeLengthPrefixSize(int length)
        {
            return ComputeVarIntSize((ulong)length);
        }

        [BurstDiscard]
        public static int ComputeLengthPrefixedStringSize(string value)
        {
            var bytesLength = Encoding.UTF8.GetByteCount(value);
            return ComputeLengthPrefixSize(bytesLength) + bytesLength;
        }
        
        public static int ComputeLengthPrefixedFixedStringSize<T>(ref T value) where T : unmanaged, IUTF8Bytes, IIndexable<byte>
        {
            return ComputeLengthPrefixSize(value.Length) + value.Length;
        }

        public static int ComputeLengthPrefixedBytesSize(ref NativeList<byte> bytes)
        {
            return ComputeLengthPrefixSize(bytes.Length) + bytes.Length;
        }

        public static int ComputeLengthPrefixedBytesSize(ref NativeArray<byte> bytes)
        {
            return ComputeLengthPrefixSize(bytes.Length) + bytes.Length;
        }

        public static int ComputeLengthPrefixedMessageSize<T>(ref T msg) where T : unmanaged, IProtoBurstMessage
        {
            var length = msg.ComputeSize();
            return ComputeLengthPrefixSize(length) + length;
        }

        public static unsafe void WriteBytes(this BufferWriter bufferWriter, ref NativeList<byte> bytes)
        {
            bufferWriter.WriteBytes(bytes.GetUnsafeReadOnlyPtr(), bytes.Length);
        }

        public static unsafe void WriteBytes(this BufferWriter bufferWriter, ref NativeArray<byte> bytes)
        {
            bufferWriter.WriteBytes((byte*)bytes.GetUnsafeReadOnlyPtr(), bytes.Length);
        }

        public static void WriteVarInt(this BufferWriter bufferWriter, ulong value)
        {
            if (value < 128UL)
            {
                bufferWriter.WriteByte((byte)value);
            }
            else
            {
                while (true)
                {
                    if (value > (ulong)sbyte.MaxValue)
                    {
                        // Add the continuation bit
                        bufferWriter.WriteByte((byte)(value & (ulong)sbyte.MaxValue | 128UL));
                        value >>= 7;
                    }
                    else
                    {
                        bufferWriter.WriteByte((byte)value);
                        return;
                    }
                }
            }
        }

        public static void WriteUFixed32(this BufferWriter bufferWriter, uint value)
        {
            bufferWriter.WriteLittleEndian32(value);
        }

        public static void WriteUFixed64(this BufferWriter bufferWriter, ulong value)
        {
            bufferWriter.WriteLittleEndian64(value);
        }

        public static void WriteSFixed32(this BufferWriter bufferWriter, int value)
        {
            bufferWriter.WriteLittleEndian32((uint)value);
        }

        public static void WriteSFixed64(this BufferWriter bufferWriter, long value)
        {
            bufferWriter.WriteLittleEndian64((ulong)value);
        }

        public static void WriteBool(this BufferWriter bufferWriter, bool value)
        {
            bufferWriter.WriteByte(value ? (byte)1 : (byte)0);
        }

        public static void WriteDouble(this BufferWriter bufferWriter, double value)
        {
            var x = math.asulong(value);
            bufferWriter.WriteLittleEndian64(x);
        }

        public static void WriteFloat(this BufferWriter bufferWriter, float value)
        {
            var x = math.asuint(value);
            bufferWriter.WriteLittleEndian32(x);
        }

        public static void WriteUInt32(this BufferWriter bufferWriter, uint value)
        {
            bufferWriter.WriteVarInt(value);
        }

        public static void WriteUInt64(this BufferWriter bufferWriter, ulong value)
        {
            bufferWriter.WriteVarInt(value);
        }

        public static void WriteInt32(this BufferWriter bufferWriter, int value)
        {
            bufferWriter.WriteVarInt((uint)value);
        }

        public static void WriteInt64(this BufferWriter bufferWriter, long value)
        {
            bufferWriter.WriteVarInt((ulong)value);
        }

        public static void WriteTag(this BufferWriter bufferWriter, uint tag)
        {
            bufferWriter.WriteVarInt(tag);
        }

        public static void WriteTag(this BufferWriter bufferWriter, int fieldNumber, WireFormat.WireType wireType)
        {
            bufferWriter.WriteVarInt(WireFormat.MakeTag(fieldNumber, wireType));
        }
        
        public static void WriteEnum(this BufferWriter bufferWriter, int value)
        {
            bufferWriter.WriteVarInt((uint)value);
        }

        public static void WriteLength(this BufferWriter bufferWriter, int length)
        {
            bufferWriter.WriteVarInt((ulong)length);
        }

        public static void WriteLengthPrefixedFixedString<T>(this BufferWriter bufferWriter, ref T value)
            where T : unmanaged, IUTF8Bytes, IIndexable<byte>
        {
            bufferWriter.WriteLength(value.Length);

            unsafe
            {
                bufferWriter.WriteBytes(value.GetUnsafePtr(), value.Length);
            }
        }

        [BurstDiscard]
        // ReSharper restore Unity.ExpensiveCode
        public static void WriteLengthPrefixedString(this BufferWriter bufferWriter, string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            bufferWriter.WriteLength(bytes.Length);
            unsafe
            {
                fixed (byte* ptr = bytes)
                {
                    bufferWriter.WriteBytes(ptr, bytes.Length);
                }
            }
        }

        public static void WriteFixedString<T>(this BufferWriter bufferWriter, ref T value)
            where T : unmanaged, IUTF8Bytes, IIndexable<byte>
        {
            unsafe
            {
                bufferWriter.WriteBytes(value.GetUnsafePtr(), value.Length);
            }
        }

        [BurstDiscard]
        // ReSharper restore Unity.ExpensiveCode
        public static void WriteString(this BufferWriter bufferWriter, string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            unsafe
            {
                fixed (byte* ptr = bytes)
                {
                    bufferWriter.WriteBytes(ptr, bytes.Length);
                }
            }
        }

        [BurstDiscard]
        public static void WriteLengthPrefixedMessage(this BufferWriter bufferWriter, ref IMessage message)
        {
            var bytes = message.ToByteArray();
            
            bufferWriter.WriteLength(bytes.Length);

            unsafe
            {
                fixed(byte* ptr = bytes)
                {
                    bufferWriter.WriteBytes(ptr, bytes.Length);
                }
            }
        }
        
        public static void WriteLengthPrefixedMessage<T>(this BufferWriter bufferWriter, ref T message)
            where T : IProtoBurstMessage
        {
            var size = message.ComputeSize();
            bufferWriter.WriteLength(size);
            message.WriteTo(ref bufferWriter);
        }

        public static void WriteLengthPrefixedBytes(this BufferWriter bufferWriter, ref NativeList<byte> bytes)
        {
            bufferWriter.WriteLength(bytes.Length);
            bufferWriter.WriteBytes(ref bytes);
        }

        public static void WriteLengthPrefixedBytes(this BufferWriter bufferWriter, ref NativeArray<byte> bytes)
        {
            bufferWriter.WriteLength(bytes.Length);
            bufferWriter.WriteBytes(ref bytes);
        }

        public static unsafe void WriteLengthPrefixedBytes(this BufferWriter bufferWriter, byte* bytes, int length)
        {
            bufferWriter.WriteLength(length);
            bufferWriter.WriteBytes(bytes, length);
        }
    }
}