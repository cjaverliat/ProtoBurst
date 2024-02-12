using System;
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

        private Any(NativeArray<byte> msgBytes, FixedString128Bytes msgTypeUrl)
        {
            _msgBytes = msgBytes;
            _msgTypeUrl = msgTypeUrl;
        }

        public static Any Pack<T>(Allocator allocator, T msg) where T : struct, IProtoBurstMessage
        {
            var msgBytes = new NativeList<byte>(allocator);
            WritingPrimitives.WriteMessage(ref msg, ref msgBytes);
            return new Any(msgBytes.AsArray(), msg.TypeUrl);
        }
        
        public static Any Pack(NativeArray<byte> msgBytes, FixedString128Bytes msgTypeUrl) 
        {
            return new Any(msgBytes, msgTypeUrl);
        }

        public void WriteTo(ref NativeList<byte> data)
        {
            if (_msgTypeUrl.Length != 0)
            {
                WritingPrimitives.WriteTag(Google.Protobuf.WellKnownTypes.Any.TypeUrlFieldNumber,
                    WireFormat.WireType.LengthDelimited, ref data);
                WritingPrimitives.WriteFixedString128Bytes(ref _msgTypeUrl, ref data);
            }

            WritingPrimitives.WriteTag(Google.Protobuf.WellKnownTypes.Any.ValueFieldNumber,
                WireFormat.WireType.LengthDelimited, ref data);
            WritingPrimitives.WriteRawBytes(ref _msgBytes, ref data);
        }

        public void Dispose()
        {
            _msgBytes.Dispose();
        }
    }
}