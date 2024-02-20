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

        private NativeList<byte> _valueBytes;
        private NativeList<byte> _typeUrlBytes;

        public static readonly uint TypeUrlTag = WireFormat.MakeTag(1, WireFormat.WireType.LengthDelimited);
        public static readonly uint ValueTag = WireFormat.MakeTag(2, WireFormat.WireType.LengthDelimited);

        private Any(NativeList<byte> valueBytes, NativeList<byte> typeUrlBytes)
        {
            _valueBytes = valueBytes;
            _typeUrlBytes = typeUrlBytes;
        }

        public static Any Pack(ReadOnlySpan<byte> valueBytes, ReadOnlySpan<byte> typeUrlBytes, Allocator allocator)
        {
            var valueBytesList = new NativeList<byte>(valueBytes.Length, allocator);
            var typeUrlBytesList = new NativeList<byte>(typeUrlBytes.Length, allocator);
            valueBytesList.ResizeUninitialized(valueBytes.Length);
            typeUrlBytesList.ResizeUninitialized(typeUrlBytes.Length);
            valueBytes.CopyTo(valueBytesList.AsArray().AsSpan());
            typeUrlBytes.CopyTo(typeUrlBytesList.AsArray().AsSpan());
            return new Any(valueBytesList, typeUrlBytesList);
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
            _valueBytes.Dispose();
            _typeUrlBytes.Dispose();
        }
    }
}