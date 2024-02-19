using System;
using Google.Protobuf;
using ProtoBurst.Packages.ProtoBurst.Runtime;
using Unity.Burst;
using Unity.Collections;

namespace ProtoBurst.Message
{
    [BurstCompile]
    public struct Any : IProtoBurstMessage, IDisposable
    {
        private static readonly FixedString128Bytes TypeUrl = "type.googleapis.com/google.protobuf.Any";

        private NativeArray<byte> _msgBytes;
        private NativeArray<byte> _msgTypeUrlBytes;

        private static readonly uint TypeUrlTag = WireFormat.MakeTag(1, WireFormat.WireType.LengthDelimited);
        private static readonly uint ValueTag = WireFormat.MakeTag(2, WireFormat.WireType.LengthDelimited);

        private Any(NativeArray<byte> msgBytes, NativeArray<byte> msgTypeUrlBytes)
        {
            _msgBytes = msgBytes;
            _msgTypeUrlBytes = msgTypeUrlBytes;
        }

        public static Any Pack<T>(T msg, Allocator allocator) where T : unmanaged, IProtoBurstMessage
        {
            var msgBytes = new BufferWriter(msg.ComputeSize(), allocator);
            msg.WriteTo(ref msgBytes);
            return new Any(msgBytes.AsArray(), msg.GetTypeUrl(allocator));
        }

        [BurstDiscard]
        public static Any Pack(IMessage msg, Allocator allocator)
        {
            var bytes = new NativeArray<byte>(msg.ToByteArray(), allocator);
            return new Any(bytes, SampleTypeUrl.Alloc(msg.Descriptor, allocator));
        }

        public static Any Pack(ReadOnlySpan<byte> value, SampleTypeUrl typeUrl, Allocator allocator)
        {
            var bytes = new NativeArray<byte>(value.Length, allocator);
            value.CopyTo(bytes);
            return new Any(bytes, typeUrl);
        }

        public static Any Pack(NativeArray<byte> value, SampleTypeUrl typeUrl)
        {
            return new Any(value, typeUrl);
        }

        public int ComputeSize()
        {
            return BufferExtensions.TagSize + BufferExtensions.ComputeLengthPrefixedBytesSize(ref _msgTypeUrlBytes) +
                   BufferExtensions.TagSize + BufferExtensions.ComputeLengthPrefixedBytesSize(ref _msgBytes);
        }

        public static int ComputeSize(int msgTypeUrlLength, int msgLength)
        {
            return BufferExtensions.TagSize + BufferExtensions.ComputeLengthPrefixSize(msgTypeUrlLength) +
                   BufferExtensions.TagSize + BufferExtensions.ComputeLengthPrefixSize(msgLength) +
                   msgTypeUrlLength + msgLength;
        }

        public void WriteTo(ref BufferWriter bufferWriter)
        {
            bufferWriter.WriteTag(TypeUrlTag);
            bufferWriter.WriteLengthPrefixedBytes(ref _msgTypeUrlBytes);
            bufferWriter.WriteTag(ValueTag);
            bufferWriter.WriteLengthPrefixedBytes(ref _msgBytes);
        }

        public static void WriteTo<T>(ref SampleTypeUrl typeUrl, ref T message, ref BufferWriter bufferWriter) where T : unmanaged, IProtoBurstMessage
        {
            var typeUrlBytes = typeUrl.Bytes;
            bufferWriter.WriteTag(TypeUrlTag);
            bufferWriter.WriteLengthPrefixedBytes(ref typeUrlBytes);
            bufferWriter.WriteTag(ValueTag);
            bufferWriter.WriteLengthPrefixedMessage(ref message);
        }
        
        [BurstDiscard]
        public static void WriteTo(SampleTypeUrl typeUrl, IMessage message, ref BufferWriter bufferWriter)
        {
            unsafe
            {
                var typeUrlBytes = typeUrl.Bytes;
                bufferWriter.WriteTag(TypeUrlTag);
                bufferWriter.WriteLengthPrefixedBytes(ref typeUrlBytes);
                bufferWriter.WriteTag(ValueTag);
                var bytes = message.ToByteArray();
                bufferWriter.WriteLength(bytes.Length);
            
                fixed(byte* bytesPtr = bytes)
                    bufferWriter.WriteBytes(bytesPtr, bytes.Length);
            }
        }

        public SampleTypeUrl GetTypeUrl(Allocator allocator)
        {
            return SampleTypeUrl.Alloc(TypeUrl, allocator);
        }

        public void Dispose()
        {
            _msgBytes.Dispose();
        }
    }
}