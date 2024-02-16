using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;

namespace ProtoBurst.Message
{
    [BurstCompile]
    public struct Any : IProtoBurstMessage, IDisposable
    {
        private static readonly FixedString128Bytes AnyTypeUrl = "type.googleapis.com/google.protobuf.Any";

        public FixedString128Bytes TypeUrl => AnyTypeUrl;

        private NativeArray<byte> _msgBytes;
        private FixedString128Bytes _msgTypeUrl;

        public Any(NativeArray<byte> msgBytes, FixedString128Bytes msgTypeUrl)
        {
            _msgBytes = msgBytes;
            _msgTypeUrl = msgTypeUrl;
        }

        public static Any Pack<T>(Allocator allocator, T msg) where T : unmanaged, IProtoBurstMessage
        {
            var tmpMsgBytes = new NativeList<byte>(msg.ComputeMaxSize(), Allocator.TempJob);
            msg.WriteToNoResize(ref tmpMsgBytes);
            var msgBytes = tmpMsgBytes.ToArray(allocator);
            tmpMsgBytes.Dispose();
            return new Any(msgBytes, msg.TypeUrl);
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