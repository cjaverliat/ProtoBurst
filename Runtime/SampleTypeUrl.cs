using System;
using Unity.Collections;

namespace ProtoBurst.Packages.ProtoBurst.Runtime
{
    public struct SampleTypeUrl : IDisposable
    {
        public int Id;
        public NativeArray<byte> Bytes;

        private SampleTypeUrl(int id, NativeArray<byte> bytes)
        {
            Id = id;
            Bytes = bytes;
        }

        // ReSharper restore Unity.ExpensiveCode
        internal static SampleTypeUrl Create(int id, string typeUrl, Allocator allocator)
        {
            var bytes = new NativeList<byte>(Allocator.TempJob);
            WritingPrimitives.WriteLengthPrefixedString(typeUrl, bytes);
            var sampleTypeUrl = new SampleTypeUrl(id, new NativeArray<byte>(bytes.AsArray(), allocator));
            bytes.Dispose();
            return sampleTypeUrl;
        }

        internal static SampleTypeUrl Create<T>(int id, T typeUrl, Allocator allocator)
            where T : unmanaged, IUTF8Bytes, IIndexable<byte>
        {
            var bytes = new NativeList<byte>(Allocator.TempJob);
            WritingPrimitives.WriteLengthPrefixedFixedString(ref typeUrl, ref bytes);
            var sampleTypeUrl = new SampleTypeUrl(id, new NativeArray<byte>(bytes.AsArray(), allocator));
            bytes.Dispose();
            return sampleTypeUrl;
        }
        
        public int BytesLength => Bytes.Length;
        
        public NativeArray<byte> AsArray() => Bytes;
        
        public static implicit operator NativeArray<byte>(SampleTypeUrl sampleTypeUrl) => sampleTypeUrl.AsArray();
        
        public void Dispose()
        {
            Bytes.Dispose();
        }
    }
}