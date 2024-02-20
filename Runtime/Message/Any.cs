using System;
using ProtoBurst.Packages.ProtoBurst.Runtime;
using Unity.Burst;
using Unity.Collections;

namespace ProtoBurst.Message
{
    [BurstCompile]
    public struct Any : IProtoBurstMessage, IDisposable
    {
        private static readonly FixedString128Bytes TypeUrl = "type.googleapis.com/google.protobuf.Any";

        private NativeArray<byte> _value;
        private NativeArray<byte> _typeUrl;

        public static readonly uint TypeUrlTag = WireFormat.MakeTag(1, WireFormat.WireType.LengthDelimited);
        public static readonly uint ValueTag = WireFormat.MakeTag(2, WireFormat.WireType.LengthDelimited);

        private Any(NativeArray<byte> value, NativeArray<byte> typeUrl)
        {
            _value = value;
            _typeUrl = typeUrl;
        }

        public static Any Pack(ReadOnlySpan<byte> value, ReadOnlySpan<byte> typeUrlBytes, Allocator allocator)
        {
            var valueArr = new NativeArray<byte>(value.Length, allocator);
            var typeUrlArr = new NativeArray<byte>(typeUrlBytes.Length, allocator);
            value.CopyTo(valueArr);
            typeUrlBytes.CopyTo(typeUrlArr);
            return new Any(valueArr, typeUrlArr);
        }

        public int ComputeSize()
        {
            return BufferExtensions.ComputeTagSize(TypeUrlTag) +
                   BufferExtensions.ComputeLengthPrefixedBytesSize(ref _typeUrl) +
                   BufferExtensions.ComputeTagSize(ValueTag) +
                   BufferExtensions.ComputeLengthPrefixedBytesSize(ref _value);
        }

        public void WriteTo(ref BufferWriter bufferWriter)
        {
            bufferWriter.WriteTag(TypeUrlTag);
            bufferWriter.WriteLengthPrefixedBytes(ref _typeUrl);
            bufferWriter.WriteTag(ValueTag);
            bufferWriter.WriteLengthPrefixedBytes(ref _value);
        }

        public static int ComputeSize(int typeUrlBytesLength, int valueBytesLength)
        {
            return BufferExtensions.ComputeTagSize(TypeUrlTag) +
                   BufferExtensions.ComputeLengthPrefixSize(typeUrlBytesLength) + typeUrlBytesLength +
                   BufferExtensions.ComputeTagSize(ValueTag) +
                   BufferExtensions.ComputeLengthPrefixSize(valueBytesLength) + valueBytesLength;
        }

        public static unsafe void WriteTo(
            byte* typeUrlBytesPtr, int typeUrlBytesLength,
            byte* valueBytesPtr, int valueBytesLength,
            ref BufferWriter bufferWriter)
        {
            bufferWriter.WriteTag(TypeUrlTag);
            bufferWriter.WriteLengthPrefixedBytes(typeUrlBytesPtr, typeUrlBytesLength);
            bufferWriter.WriteTag(ValueTag);
            bufferWriter.WriteLengthPrefixedBytes(valueBytesPtr, valueBytesLength);
        }

        public SampleTypeUrl GetTypeUrl(Allocator allocator)
        {
            return SampleTypeUrl.Alloc(TypeUrl, allocator);
        }

        public void Dispose()
        {
            _value.Dispose();
        }
    }
}