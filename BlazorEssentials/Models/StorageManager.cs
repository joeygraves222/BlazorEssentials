using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazorEssentials.Models
{
    public interface IStorageManager
    {
        Task<T> GetLocalAsync<T>(string key);
        Task<T> GetSessionAsync<T>(string key);
        Task DeleteLocalAsync(string key);
        Task DeleteSessionAsync(string key);
        Task SetLocalAsync<T>(string key, T item);
        Task SetSessionAsync<T>(string key, T item);
        Task DeleteAllLocalAsync();
        Task DeleteAllSessionAsync();
    }
    public class StorageManager : IStorageManager
    {
        private IJSRuntime JS { get; set; }
        private Interop interop { get; set; }
        public StorageManager(IJSRuntime js)
        {
            JS = js;
            interop = new(js);
        }

        public async Task DeleteLocalAsync(string key)
        {
            await interop.DeleteLocalStorage(key);
        }

        public async Task DeleteSessionAsync(string key)
        {
            await interop.DeleteSessionStorage(key);
        }

        public async Task<T> GetLocalAsync<T>(string key)
        {
            try
            {
                var raw = await interop.GetLocalStorage(key);
                return JsonSerializer.Deserialize<T>(raw);
            }
            catch (Exception ex)
            {
                return default;
            }
        }

        public async Task<T> GetSessionAsync<T>(string key)
        {
            try
            {
                var raw = await interop.GetSessionStorage(key);
                return JsonSerializer.Deserialize<T>(raw);
            }
            catch (Exception ex)
            {
                return default;
            }
        }

        public async Task SetLocalAsync<T>(string key, T item)
        {
            var serialized = JsonSerializer.Serialize(item);
            await interop.SetLocalStorage(key, serialized);
        }

        public async Task SetSessionAsync<T>(string key, T item)
        {
            var serialized = JsonSerializer.Serialize(item);
            await interop.SetSessionStorage(key, serialized);
        }

        public async Task DeleteAllLocalAsync()
        {
            await interop.DeleteAllLocalStorage();
        }

        public async Task DeleteAllSessionAsync()
        {
            await interop.DeleteAllSessionStorage();
        }
    }
}
