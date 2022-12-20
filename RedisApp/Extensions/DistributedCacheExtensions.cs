using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace RedisApp.Extensions
{
    public static class DistributedCacheExtensions
    {
        public static async Task SetRecordAsync<T>(this IDistributedCache cache,
            string recordId,
            T data,
            TimeSpan? absoluteExpireTime = null,
            TimeSpan? unusedExpireTime = null)
        {

            var options = new DistributedCacheEntryOptions();

            //set lifetime of data - max
            options.AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromSeconds(60);

            //remove data if not accessed within this time
            options.SlidingExpiration = unusedExpireTime;

            //convert into json string
            var jsonData = JsonSerializer.Serialize(data);

            await cache.SetStringAsync(recordId, jsonData, options);
        }


        public static async Task<T> GetRecordAsync<T>(this IDistributedCache cache,
            string recordId)
        { 
            var jsonData = await cache.GetStringAsync(recordId);

            if (jsonData is null)
            {
                return default(T);
            }

            return JsonSerializer.Deserialize<T>(jsonData);
        }
    }
}
