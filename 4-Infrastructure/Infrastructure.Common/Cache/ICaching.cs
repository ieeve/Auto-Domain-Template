namespace Infrastructure.Common.Cache
{
    public interface ICaching
    {
        T GetOrSetObjectFromCache<T>(string cacheItemName, int cacheTimeInMinutes, Func<T> objectSettingFunction);

        Task<T> GetOrSetObjectFromCacheAsync<T>(string cacheItemName, int cacheTimeInMinutes, Func<Task<T>> objectSettingFunction);

        void Invalidate(string key);
        bool ContainsKey(string key);
        void InvalidateAll();
        IEnumerable<string> GetAllKey();
        void SetValueToCache(string key, object value, int cacheTimeInMinutes = 120);

        object GetValueFromCache(string key);
        bool TryGetValueFromCache(string key, out object result);
    }
}
