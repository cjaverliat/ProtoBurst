using System;
using System.Runtime.CompilerServices;
using Google.Protobuf;
using Unity.Burst;
using Unity.Collections;

namespace ProtoBurst.Message
{
    [BurstCompile]
    public struct Any : IProtoBurstMessage, IDisposable
    {
        private static readonly FixedString128Bytes AnyTypeUrl = "type.googleapis.com/" + Google.Protobuf.WellKnownTypes.Any.Descriptor.FullName;

        public FixedString128Bytes TypeUrl => AnyTypeUrl;

        private NativeArray<byte> _msgBytes;
        private FixedString128Bytes _msgTypeUrl;

        private Any(NativeArray<byte> msgBytes, FixedString128Bytes msgTypeUrl)
        {
            _msgBytes = msgBytes;
            _msgTypeUrl = msgTypeUrl;
        }

        public static Any Pack<T>(T msg, Allocator allocator) where T : unmanaged, IProtoBurstMessage
        {
            var tmpMsgBytes = new NativeList<byte>(msg.ComputeMaxSize(), Allocator.Persistent);
            msg.WriteToNoResize(ref tmpMsgBytes);
            var msgBytes = tmpMsgBytes.ToArray(allocator);
            tmpMsgBytes.Dispose();
            return new Any(msgBytes, msg.TypeUrl);
        }
        
        [BurstDiscard]
        public static Any Pack(IMessage msg, Allocator allocator)
        {
            var bytes = new NativeArray<byte>(msg.ToByteArray(), allocator);
            var typeUrl = new FixedString128Bytes(msg.Descriptor.FullName);
            return new Any(bytes, typeUrl);
        }

        public static Any Pack(ReadOnlySpan<byte> msgBytes, FixedString128Bytes msgTypeUrl, Allocator allocator)
        {
            var bytes = new NativeArray<byte>(msgBytes.Length, allocator);
            msgBytes.CopyTo(bytes);
            return new Any(bytes, msgTypeUrl);
        }
        
        public static Any Pack(NativeArray<byte> msgBytes, FixedString128Bytes msgTypeUrl)
        {
            return new Any(msgBytes, msgTypeUrl);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ComputeMaxSize()
        {
            var msgTypeUrl = WritingPrimitives.TagSize + WritingPrimitives.LengthPrefixMaxSize + _msgTypeUrl.Length;
            var msgBytes = WritingPrimitives.TagSize + WritingPrimitives.LengthPrefixMaxSize + _msgBytes.Length;
            return msgTypeUrl + msgBytes;
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteToNoResize(ref NativeList<byte> data)
        {
            if (_msgTypeUrl.Length != 0)
            {
                WritingPrimitives.WriteTagNoResize(Google.Protobuf.WellKnownTypes.Any.TypeUrlFieldNumber,
                    WireFormat.WireType.LengthDelimited, ref data);
                WritingPrimitives.WriteLengthPrefixedFixedStringNoResize(ref _msgTypeUrl, ref data);
            }

            WritingPrimitives.WriteTagNoResize(Google.Protobuf.WellKnownTypes.Any.ValueFieldNumber,
                WireFormat.WireType.LengthDelimited, ref data);
            WritingPrimitives.WriteLengthPrefixedBytesNoResize(ref _msgBytes, ref data);
        }

        public void Dispose()
        {
            _msgBytes.Dispose();
        }
    }
}