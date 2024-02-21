using System;
using Google.Protobuf.Reflection;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;

namespace ProtoBurst.Packages.ProtoBurst.Runtime
{
    [BurstCompile]
    public struct SampleTypeUrl : IDisposable
    {
        private NativeList<byte> _bytes;

        private SampleTypeUrl(NativeList<byte> bytes)
        {
            _bytes = bytes;
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
            var length = BufferWriterExtensions.ComputeStringSize(typeUrl);
            var bytes = new NativeList<byte>(length, allocator);
            var buffer = new BufferWriter(bytes);
            buffer.WriteString(typeUrl);
            return new SampleTypeUrl(bytes);
        }

        private static SampleTypeUrl AllocInternal<T>(T typeUrl, Allocator allocator)
            where T : unmanaged, IUTF8Bytes, INativeList<byte>
        {
            var length = BufferWriterExtensions.ComputeFixedStringSize(ref typeUrl);
            var bytes = new NativeList<byte>(length, allocator);
            var buffer = new BufferWriter(bytes);
            buffer.WriteFixedString(ref typeUrl);
            return new SampleTypeUrl(bytes);
        }

        public int BytesLength => _bytes.Length;

        public NativeArray<byte> AsArray() => _bytes.AsArray();
        
        public NativeList<byte> AsList() => _bytes;

        public ReadOnlySpan<byte> AsReadOnlySpan() => _bytes.AsArray().AsReadOnlySpan();
        
        public static implicit operator NativeArray<byte>(SampleTypeUrl sampleTypeUrl) => sampleTypeUrl.AsArray();
        
        public static implicit operator NativeList<byte>(SampleTypeUrl sampleTypeUrl) => sampleTypeUrl.AsList();
        
        public static implicit operator ReadOnlySpan<byte>(SampleTypeUrl sampleTypeUrl) => sampleTypeUrl.AsReadOnlySpan();

        public void Dispose()
        {
            _bytes.Dispose();
        }
    }
}