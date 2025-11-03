using System.Runtime.CompilerServices;
using System.Text.Json;

using RedisClient.Interfaces;

using StackExchange.Redis;

namespace RedisClient.Implementations;

internal sealed class RedisCache : IRedisCache
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _database;

    public RedisCache(IConnectionMultiplexer redis)
    {
        _redis = redis;
        _database = redis.GetDatabase();
    }

    public async Task<T> GetAsync<T>(string key)
    {
        var value = await _database.StringGetAsync(key);
        if (value.IsNullOrEmpty || value == RedisValue.Null)
        {
            throw new KeyNotFoundException($"Key '{key}' was not found in Redis.");
        }

        return JsonSerializer.Deserialize<T>(value.ToString()!)!;
    }

    public async Task<IList<T>> GetAsync<T>(string[] keys)
    {
        RedisKey[] redisKeys = Array.ConvertAll(keys, key => (RedisKey)key);
        var result = new Dictionary<string, T>();
        await FillBatchAsync(redisKeys, result);

        return result.Values.ToList();
    }

    public async Task<(bool Found, T? Value)> TryGetAsync<T>(string key)
    {
        var value = await _database.StringGetAsync(key);
        if (value.IsNullOrEmpty || value == RedisValue.Null)
        {
            return (false, default);
        }

        return (true, JsonSerializer.Deserialize<T>(value.ToString()!)!);
    }

    public async Task<IDictionary<string, T>> GetByPatternAsync<T>(
        string pattern,
        int pageSize = 1000,
        CancellationToken cancellationToken = default)
    {
        var result = new Dictionary<string, T>();
        var keys = new List<RedisKey>(pageSize);

        await foreach (var key in ScanKeysAsync(pattern, pageSize, cancellationToken))
        {
            keys.Add(key);
            if (keys.Count < pageSize) continue;

            await FillBatchAsync(keys, result);
            keys.Clear();
        }

        if (keys.Count > 0) await FillBatchAsync(keys, result);

        return result;
    }

    public async Task<bool> ExistsAsync(string key)
    {
        return await _database.KeyExistsAsync(key);
    }

    public async Task<long> ExistsAsync(string[] keys)
    {
        RedisKey[] redisKeys = Array.ConvertAll(keys, key => (RedisKey)key);
        return await _database.KeyExistsAsync(redisKeys);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var serializedValue = JsonSerializer.Serialize(value);
        await _database.StringSetAsync(key, serializedValue, expiry);
    }

    public async Task SetAsync<T>(IDictionary<string, T> keyValues)
    {
        var pairs = keyValues
            .Select(keyValue => new KeyValuePair<RedisKey, RedisValue>(
                keyValue.Key,
                JsonSerializer.Serialize(keyValue.Value)))
            .ToArray();

        await _database.StringSetAsync(pairs);
    }

    public async Task RemoveAsync(string key)
    {
        await _database.KeyDeleteAsync(key);
    }

    public async Task RemoveAsync(string[] keys)
    {
        RedisKey[] redisKeys = Array.ConvertAll(keys, key => (RedisKey)key);
        await _database.KeyDeleteAsync(redisKeys);
    }

    public async Task<long> RemoveByPatternAsync(
        string pattern,
        int pageSize = 1000,
        CancellationToken cancellationToken = default)
    {
        long deleted = 0;
        var keysToDelete = new List<RedisKey>(pageSize);

        await foreach (var key in ScanKeysAsync(pattern, pageSize, cancellationToken))
        {
            keysToDelete.Add(key);
            if (keysToDelete.Count < pageSize) continue;

            deleted += await _database.KeyDeleteAsync(keysToDelete.ToArray());
            keysToDelete.Clear();
        }

        if (keysToDelete.Count > 0)
        {
            deleted += await _database.KeyDeleteAsync(keysToDelete.ToArray());
        }

        return deleted;
    }

    private async IAsyncEnumerable<RedisKey> ScanKeysAsync(
        string pattern,
        int pageSize,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var endpoint in _redis.GetEndPoints())
        {
            var server = _redis.GetServer(endpoint);
            if (!server.IsConnected) continue;

            var keys = server.KeysAsync(pattern: pattern, pageSize: pageSize);
            await foreach (var key in keys.WithCancellation(cancellationToken))
            {
                yield return key;
            }
        }
    }

    private async Task FillBatchAsync<T>(IReadOnlyList<RedisKey> keys, IDictionary<string, T> target)
    {
        if (keys.Count == 0) return;

        var values = await _database.StringGetAsync(keys.ToArray());

        for (var i = 0; i < keys.Count; i++)
        {
            if (values[i].IsNull) continue;

            var value = JsonSerializer.Deserialize<T>(values[i]!);
            if (value is null) continue;

            var keyString = (string)keys[i]!;
            target[keyString] = value;
        }
    }
}
