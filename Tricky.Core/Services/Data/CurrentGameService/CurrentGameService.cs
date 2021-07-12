using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Tricky.Core.Services.Data.CurrentGameService
{
    public class CurrentGameService
    {
        public IDatabase Database { get; set; }

        public CurrentGameService() { }

        public CurrentGameService(string redisHost)
        {
            Database = ConnectionMultiplexer
            .Connect(ConfigurationOptions.Parse(redisHost))
            .GetDatabase();
        }

        public virtual async Task Delete(string key)
        {
            await Database.KeyDeleteAsync(key);
        }

        public virtual async Task<bool> Exists(string key)
        {
            bool response = await Database.KeyExistsAsync(key);
            return response;
        }

        public virtual async Task<string> Get(string key)
        {
            string response = await Database.StringGetAsync(key);
            return response;
        }

        public virtual async Task Set(string key, string value, TimeSpan ttl)
        {
            await Database.StringSetAsync(key, value, ttl);
        }

        public virtual async Task Increment(string key)
        {
            await Database.StringIncrementAsync(key, 1);
        }
    }
}
