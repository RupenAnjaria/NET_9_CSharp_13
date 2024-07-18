using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console9
{
    public class HCaching
    {
        private const string CacheKey = "mykey";
        private readonly IServiceProvider _serviceProvider;
        private readonly IDistributedCache _distributedCache;
        private readonly IMemoryCache _memoryCache;
        private readonly HybridCache hybridCache;
        
        
        public HCaching()
        {
            var services = new ServiceCollection();
            services.AddDistributedMemoryCache();
            services.AddHybridCache();
            services.AddMemoryCache();

            _serviceProvider = services.BuildServiceProvider();
            _distributedCache = _serviceProvider.GetService<IDistributedCache>();
            _memoryCache = _serviceProvider.GetService<IMemoryCache>();
            hybridCache = _serviceProvider.GetService<HybridCache>();

        }

        private async Task<string> GetDataAsyc()
        {
            await Task.Delay(1000);
            return "Data from GetDataAsyc";
        }

        public async Task<string> NoCache()
        {
            Console.WriteLine("...NoCache...");
            return await GetDataAsyc();
        }

        public async Task<string> GetFromDistributedCache()
        {
            var data = await _distributedCache.GetStringAsync(CacheKey);
            if (data == null) {
                data = await GetDataAsyc();
                await _distributedCache.SetStringAsync(CacheKey, data);
            }
            return data;
        }

        public async Task<string> GetFromMemoryCache() {
            if (!_memoryCache.TryGetValue(CacheKey, out string data)) { 
                data = await GetDataAsyc();
                _memoryCache.Set(CacheKey, data);
            }
            return data;
        }
        
        public async Task<string> GetFromHybridCache()
        {

            return await hybridCache.GetOrCreateAsync(
                CacheKey,
                async token => await GetDataAsyc(),
                token: default
                );
        }
    }
}
