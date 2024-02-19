using System;
using Google.Protobuf.Reflection;
using Unity.Burst;
using Unity.Collections;

namespace ProtoBurst.Packages.ProtoBurst.Runtime
{
    [BurstCompile]
    public struct SampleTypeUrl : IDisposable
    {
        public NativeArray<byte> Bytes;

        private SampleTypeUrl(NativeArray<byte> bytes)
        {
            Bytes = bytes;
        }

        // ReSharper restore Unity.ExpensiveCode
        
        [BurstDiscard]
        public static SampleTypeUrl Alloc(IDescriptor descriptor, Allocator allocator)
        {
            return Alloc("type.googleapis.com", descriptor, allocator);
        }
        
        // ReSharper restore Unity.ExpensiveCode

        [BurstDiscard]
        public static SampleTypeUrl Alloc(string prefix, IDescriptor descriptor, Allocator allocator)
        {
            var typeUrl = !prefix.EndsWith("/") ? prefix + "/" + descriptor.FullName : prefix + descriptor.FullName;
            return AllocInternal(typeUrl, allocator);
        }

        [BurstDiscard]
        public static SampleTypeUrl Alloc<TU>(TU typeUrl, Allocator allocator)
            where TU : unmanaged, IUTF8Bytes, INativeList<byte>, IIndexable<byte>
        {
            // TODO: check typeUrl is valid and contains prefix
            return AllocInternal(typeUrl, allocator);
        }

        [BurstDiscard]
        public static SampleTypeUrl Alloc<TU, TV>(TU prefix, TV descriptorFullName, Allocator allocator)
            where TU : unmanaged, IUTF8Bytes, INativeList<byte>, IIndexable<byte>
            where TV : unmanaged, IUTF8Bytes, INativeList<byte>, IIndexable<byte>
        {
            var typeUrl = new FixedString512Bytes();

            if (prefix.EndsWith('/'))
            {
                typeUrl.Append(prefix);
                typeUrl.Append(descriptorFullName);
            }
            else
            {
                typeUrl.Append(prefix);
                typeUrl.Append('/');
                typeUrl.Append(descriptorFullName);
            }

            return AllocInternal(typeUrl, allocator);
        }

        // ReSharper restore Unity.ExpensiveCode

        [BurstDiscard]
        private static SampleTypeUrl AllocInternal(string typeUrl, Allocator allocator)
        {
            var length = BufferExtensions.ComputeStringSize(typeUrl);
            var buffer = new BufferWriter(length, allocator);
            buffer.WriteString(typeUrl);
            return new SampleTypeUrl(buffer.AsArray());
        }

        private static SampleTypeUrl AllocInternal<T>(T typeUrl, Allocator allocator)
            where T : unmanaged, IUTF8Bytes, INativeList<byte>
        {
            var length = BufferExtensions.ComputeFixedStringSize(ref typeUrl);
            var buffer = new BufferWriter(length, allocator);
            buffer.WriteFixedString(ref typeUrl);
            return new SampleTypeUrl(buffer.AsArray());
        }

        public int BytesLength => Bytes.Length;

        public NativeArray<byte> AsArray() => Bytes;

        public static implicit operator NativeArray<byte>(SampleTypeUrl sampleTypeUrl) => sampleTypeUrl.AsArray();

        public void Free()
        {
            Dispose();
        }

        public void Dispose()
        {
            Bytes.Dispose();
        }
    }
}