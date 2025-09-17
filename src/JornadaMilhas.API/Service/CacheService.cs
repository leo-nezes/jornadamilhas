using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace JornadaMilhas.API.Service
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache cache;

        public CacheService(IDistributedCache cache)
        {
            this.cache = cache;
        }

        public async Task<T> GetCachedDataAsync<T>(string key)
        {
            var data = await cache.GetStringAsync(key);
            return data != null ? JsonSerializer.Deserialize<T>(data) : default;
        }

        public async Task SetCachedDataAsync<T>(string key, T data, TimeSpan expiration)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            };

            var jsonData = JsonSerializer.Serialize(data);

            await cache.SetStringAsync(key, jsonData, options);
        }

        public async Task InvalidateDataAsync(string key)
        {
            await cache.RemoveAsync(key);
        }
    }
}