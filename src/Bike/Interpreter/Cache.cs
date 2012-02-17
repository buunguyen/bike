namespace Bike.Interpreter
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Diagnostics;

    [DebuggerStepThrough]
    internal sealed class Cache<TKey, TValue>
    {
        private readonly IDictionary<TKey, WeakReference> entries;

        public Cache()
        {
            entries = new ConcurrentDictionary<TKey, WeakReference>();
        }

        public int Count
        {
            get { return ClearCollected(); }
        }

        public TValue this[TKey key]
        {
            get { return Get(key); }
            set { Insert(key, value); }
        }

        public void Insert(TKey key, TValue value)
        {
            entries[key] = new WeakReference(value);
        }

        public TValue Get(TKey key)
        {
            return (TValue)entries[key].Target;
        }

        public bool Remove(TKey key)
        {
            return entries.Remove(key);
        }

        public void Clear()
        {
            entries.Clear();
        }

        internal int ClearCollected()
        {
            IList<TKey> keys = entries.Where(kvp => !kvp.Value.IsAlive).Select(kvp => kvp.Key).ToList();
            foreach (var key in keys)
            {
                entries.Remove(key);
            }
            return entries.Count;
        }

        public override string ToString()
        {
            int count = ClearCollected();
            return count > 0 ? String.Format("Cache contains {0} live objects.", count) : "Cache is empty.";
        }
    }
}