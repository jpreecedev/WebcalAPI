namespace WebcalAPI.Core
{
    using System;
    using System.Runtime.Caching;

    public static class MemoryCacher
    {
        private static readonly object _syncLock = new object();

        public static T GetValue<T>(string key)
        {
            var memoryCache = MemoryCache.Default;
            var data = memoryCache.Get(key);
            if (data == null)
            {
                lock (_syncLock)
                {
                    data = memoryCache.Get(key);
                }
            }

            return (T) data;
        }

        public static bool Add(string key, object value, DateTimeOffset absExpiration)
        {
            var memoryCache = MemoryCache.Default;
            return memoryCache.Add(key, value, absExpiration);
        }

        public static void Delete(string key)
        {
            var memoryCache = MemoryCache.Default;
            if (memoryCache.Contains(key))
            {
                memoryCache.Remove(key);
            }
        }
    }
}