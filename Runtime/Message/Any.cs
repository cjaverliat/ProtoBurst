using Unity.Burst;
using Unity.Collections;

namespace ProtoBurst.Message
{
    [BurstCompile]
    public readonly struct Any<T> : IProtoBurstMessage where T : struct, IProtoBurstMessage
    {
        private static readonly FixedString512Bytes AnyTypeUrl = "type.googleapis.com/google.protobuf.Any";

        private readonly T _message;

        public Any(T message)
        {
            _message = message;
        }

        public void WriteTo(ref NativeList<byte> data)
        {
            var typeUrl = _message.TypeUrl;

            if (typeUrl.Length != 0)
            {
                WritingPrimitives.WriteTag(1, WireFormat.WireType.LengthDelimited, ref data);
                WritingPrimitives.WriteFixedString512Bytes(ref typeUrl, ref data);
            }

            WritingPrimitives.WriteTag(2, WireFormat.WireType.LengthDelimited, ref data);
            WritingPrimitives.WriteMessage(_message, ref data);
        }

        public FixedString512Bytes TypeUrl => AnyTypeUrl;
    }
}