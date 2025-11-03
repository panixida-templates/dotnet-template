namespace RedisClient.Interfaces;

public interface IRedisCache
{
    Task<T> GetAsync<T>(string key);
    Task<IList<T>> GetAsync<T>(string[] keys);
    Task<(bool Found, T? Value)> TryGetAsync<T>(string key);
    Task<IDictionary<string, T>> GetByPatternAsync<T>(string pattern, int pageSize = 1000, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string key);
    Task<long> ExistsAsync(string[] keys);
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
    Task SetAsync<T>(IDictionary<string, T> keyValues);
    Task RemoveAsync(string key);
    Task RemoveAsync(string[] keys);
    Task<long> RemoveByPatternAsync(string pattern, int pageSize = 1000, CancellationToken cancellationToken = default);
}
