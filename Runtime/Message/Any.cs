using System;
using System.Runtime.CompilerServices;
using Google.Protobuf;
using ProtoBurst.Packages.ProtoBurst.Runtime;
using Unity.Burst;
using Unity.Collections;

namespace ProtoBurst.Message
{
    [BurstCompile]
    public struct Any : IProtoBurstMessage, IDisposable
    {
        private static readonly SampleTypeUrl AnyTypeUrl =
            SampleTypeUrlRegistry.GetOrCreate(Google.Protobuf.WellKnownTypes.Any.Descriptor);

        public SampleTypeUrl TypeUrl => AnyTypeUrl;

        private NativeArray<byte> _msgBytes;
        private SampleTypeUrl _msgTypeUrl;

        private Any(NativeArray<byte> msgBytes, SampleTypeUrl msgTypeUrl)
        {
            _msgBytes = msgBytes;
            _msgTypeUrl = msgTypeUrl;
        }

        public static Any Pack<T>(T msg, Allocator allocator) where T : unmanaged, IProtoBurstMessage
        {
            var tmpMsgBytes = new NativeList<byte>(Allocator.Persistent);
            msg.WriteTo(ref tmpMsgBytes);
            var msgBytes = tmpMsgBytes.ToArray(allocator);
            tmpMsgBytes.Dispose();
            return new Any(msgBytes, msg.TypeUrl);
        }

        [BurstDiscard]
        public static Any Pack(IMessage msg, Allocator allocator)
        {
            var bytes = new NativeArray<byte>(msg.ToByteArray(), allocator);
            var typeUrl = SampleTypeUrlRegistry.GetOrCreate(msg.Descriptor);
            return new Any(bytes, typeUrl);
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

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ComputeMaxSize()
        {
            var msgTypeUrl = WritingPrimitives.TagSize + WritingPrimitives.LengthPrefixMaxSize +
                             _msgTypeUrl.BytesLength;
            var msgBytes = WritingPrimitives.TagSize + WritingPrimitives.LengthPrefixMaxSize + _msgBytes.Length;
            return msgTypeUrl + msgBytes;
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ComputeMaxSize(ref SampleTypeUrl sampleTypeUrl, int msgMaxSize)
        {
            var msgTypeUrl = WritingPrimitives.TagSize + WritingPrimitives.LengthPrefixMaxSize +
                             sampleTypeUrl.BytesLength;
            var msgBytes = WritingPrimitives.TagSize + WritingPrimitives.LengthPrefixMaxSize + msgMaxSize;
            return msgTypeUrl + msgBytes;
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteTo(ref NativeList<byte> data)
        {
            if (_msgTypeUrl.BytesLength != 0)
            {
                var msgTypeUrlBytes = _msgTypeUrl.AsArray();
                WritingPrimitives.WriteTag(Google.Protobuf.WellKnownTypes.Any.TypeUrlFieldNumber,
                    WireFormat.WireType.LengthDelimited, ref data);
                WritingPrimitives.WriteLengthPrefixedBytes(ref msgTypeUrlBytes, ref data);
            }

            WritingPrimitives.WriteTag(Google.Protobuf.WellKnownTypes.Any.ValueFieldNumber,
                WireFormat.WireType.LengthDelimited, ref data);
            WritingPrimitives.WriteLengthPrefixedBytes(ref _msgBytes, ref data);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteToNoResize(ref NativeList<byte> data)
        {
            if (_msgTypeUrl.BytesLength != 0)
            {
                var msgTypeUrlBytes = _msgTypeUrl.AsArray();
                WritingPrimitives.WriteTagNoResize(Google.Protobuf.WellKnownTypes.Any.TypeUrlFieldNumber,
                    WireFormat.WireType.LengthDelimited, ref data);
                WritingPrimitives.WriteLengthPrefixedBytesNoResize(ref msgTypeUrlBytes, ref data);
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