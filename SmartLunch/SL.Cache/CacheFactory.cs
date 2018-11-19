using EP.APP.Cache.RedisCache;

namespace EP.APP.Cache
{
    /// <summary>
    /// 缓存工厂
    /// </summary>
    public class CacheFactory
    {
        private static IRedisCache _cacheClient;

        /// <summary>
        /// 获取Redis缓存实例
        /// </summary>
        /// <returns></returns>
        public static IRedisCache GetRedisClient()
        {
            return _cacheClient ?? (_cacheClient = new RedisCache.RedisCache(3));
        }


    }
}
