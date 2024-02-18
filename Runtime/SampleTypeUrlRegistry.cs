using System;
using System.Collections.Concurrent;
using System.Threading;
using Google.Protobuf.Reflection;
using Unity.Collections;

namespace ProtoBurst.Packages.ProtoBurst.Runtime
{
    public class SampleTypeUrlRegistry : IDisposable
    {
        private static readonly SampleTypeUrlRegistry Instance = new();

        private int _nextId;
        private readonly ConcurrentDictionary<string, SampleTypeUrl> _sampleTypeUrls = new();
        private readonly ConcurrentDictionary<int, SampleTypeUrl> _sampleTypeUrlById = new();

        private SampleTypeUrlRegistry()
        {
        }

        ~SampleTypeUrlRegistry()
        {
            Dispose();
        }
        
        private SampleTypeUrl GetByIdInternal(int id)
        {
            return _sampleTypeUrlById[id];
        }

        // ReSharper restore Unity.ExpensiveCode

        private SampleTypeUrl GetOrCreateInternal(string typeUrl)
        {
            if (_sampleTypeUrls.TryGetValue(typeUrl, out var sampleTypeUrl))
                return sampleTypeUrl;
            var id = Interlocked.Increment(ref _nextId);
            sampleTypeUrl = SampleTypeUrl.Create(id, typeUrl, Allocator.Persistent);
            _sampleTypeUrls[typeUrl] = sampleTypeUrl;
            _sampleTypeUrlById[id] = sampleTypeUrl;
            return sampleTypeUrl;
        }

        public static SampleTypeUrl GetById(int id)
        {
            return Instance.GetByIdInternal(id);
        }

        // ReSharper restore Unity.ExpensiveCode

        public static SampleTypeUrl GetOrCreate(string typeUrl)
        {
            return Instance.GetOrCreateInternal(typeUrl);
        }

        // ReSharper restore Unity.ExpensiveCode

        public static SampleTypeUrl GetOrCreate(string prefix, IDescriptor descriptor)
        {
            var typeUrl = !prefix.EndsWith("/") ? prefix + "/" + descriptor.FullName : prefix + descriptor.FullName;
            return Instance.GetOrCreateInternal(typeUrl);
        }

        // ReSharper restore Unity.ExpensiveCode

        public static SampleTypeUrl GetOrCreate(IDescriptor descriptor)
        {
            return Instance.GetOrCreateInternal($"type.googleapis.com/{descriptor.FullName}");
        }

        public void Dispose()
        {
            foreach (var sampleTypeUrl in _sampleTypeUrls)
            {
                sampleTypeUrl.Value.Dispose();
            }

            _sampleTypeUrls.Clear();
        }
    }
}