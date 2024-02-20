using System;
using System.Runtime.CompilerServices;
using ProtoBurst.Packages.ProtoBurst.Runtime;
using Unity.Burst;
using Unity.Collections;

namespace ProtoBurst.Message
{
    [BurstCompile]
    public struct Any : IProtoBurstMessage, IDisposable
    {
        private static readonly FixedString128Bytes TypeUrl = "type.googleapis.com/google.protobuf.Any";

        private NativeList<byte> _valueBytes;
        private NativeList<byte> _typeUrlBytes;

        public static readonly uint TypeUrlTag = WireFormat.MakeTag(1, WireFormat.WireType.LengthDelimited);
        public static readonly uint ValueTag = WireFormat.MakeTag(2, WireFormat.WireType.LengthDelimited);

        private Any(NativeList<byte> valueBytes, NativeList<byte> typeUrlBytes)
        {
            _valueBytes = valueBytes;
            _typeUrlBytes = typeUrlBytes;
        }

        public static Any Pack<T>(T message, Allocator allocator) where T : unmanaged, IProtoBurstMessage
        {
            return new Any(message.ToBytes(allocator), message.GetTypeUrl(allocator));
        }

        public static Any Pack(NativeList<byte> valueBytes, NativeList<byte> typeUrlBytes, Allocator allocator)
        {
            var valueBytesCopy = new NativeList<byte>(valueBytes.Length, allocator);
            var typeUrlBytesCopy = new NativeList<byte>(typeUrlBytes.Length, allocator);
            valueBytesCopy.AddRange(valueBytes.AsArray());
            typeUrlBytesCopy.AddRange(typeUrlBytes.AsArray());
            return new Any(valueBytesCopy, typeUrlBytesCopy);
        }

        public int ComputeSize()
        {
            return BufferExtensions.ComputeTagSize(TypeUrlTag) +
                   BufferExtensions.ComputeLengthPrefixedBytesSize(ref _typeUrlBytes) +
                   BufferExtensions.ComputeTagSize(ValueTag) +
                   BufferExtensions.ComputeLengthPrefixedBytesSize(ref _valueBytes);
        }

        public void WriteTo(ref BufferWriter bufferWriter)
        {
            bufferWriter.WriteTag(TypeUrlTag);
            bufferWriter.WriteLengthPrefixedBytes(ref _typeUrlBytes);
            bufferWriter.WriteTag(ValueTag);
            bufferWriter.WriteLengthPrefixedBytes(ref _valueBytes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ComputeSize(int typeUrlBytesLength, int valueBytesLength)
        {
            return BufferExtensions.ComputeTagSize(TypeUrlTag) +
                   BufferExtensions.ComputeLengthPrefixSize(typeUrlBytesLength) + typeUrlBytesLength +
                   BufferExtensions.ComputeTagSize(ValueTag) +
                   BufferExtensions.ComputeLengthPrefixSize(valueBytesLength) + valueBytesLength;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteTo<T>(ref NativeArray<byte> typeUrlBytes, ref T message, ref BufferWriter bufferWriter)
            where T : unmanaged, IProtoBurstMessage
        {
            bufferWriter.WriteTag(TypeUrlTag);
            bufferWriter.WriteLengthPrefixedBytes(ref typeUrlBytes);
            bufferWriter.WriteTag(ValueTag);
            bufferWriter.WriteLengthPrefixedMessage(ref message);
        }

        public SampleTypeUrl GetTypeUrl(Allocator allocator)
        {
            return SampleTypeUrl.Alloc(TypeUrl, allocator);
        }

        public void Dispose()
        {
            _valueBytes.Dispose();
            _typeUrlBytes.Dispose();
        }
    }
}