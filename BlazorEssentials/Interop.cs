using BlazorEssentials.Models;
using Microsoft.JSInterop;
using System.Text.Json;

namespace BlazorEssentials
{
    // This class provides an example of how JavaScript functionality can be wrapped
    // in a .NET class for easy consumption. The associated JavaScript module is
    // loaded on demand when first needed.
    //
    // This class can be registered as scoped DI service and then injected into Blazor
    // components for use.

    public class Interop : IAsyncDisposable
    {
        private readonly Lazy<Task<IJSObjectReference>> moduleTask;

        public Interop(IJSRuntime jsRuntime)
        {
            moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/Net-BlazorEssentials/BlazorEssentialsInterop.js").AsTask());
        }

        public async ValueTask<GeoLocation> GetGeoLocation()
        {
            try
            {
                var module = await moduleTask.Value;
                var result = await module.InvokeAsync<string>("GetGeoLocation");

                var location = JsonSerializer.Deserialize<GeoLocation>(result);
                return location;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public async ValueTask<string> GetLocalStorage(string key)
        {
            try
            {
                var module = await moduleTask.Value;
                return await module.InvokeAsync<string>("GetLocalStorage", key);
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public async Task ShowModalDialog(string ModalId)
        {
            try
            {
                var module = await moduleTask.Value;
                await module.InvokeVoidAsync("showModalDialog", ModalId);
            }
            catch (Exception ex)
            {
            }
        }

        public async Task CloseModalDialog(string ModalId)
        {
            try
            {
                var module = await moduleTask.Value;
                await module.InvokeVoidAsync("closeModalDialog", ModalId);
            }
            catch (Exception ex)
            {
            }
        }



        public async Task SetLocalStorage(string key, string item)
        {
            try
            {
                var module = await moduleTask.Value;
                await module.InvokeAsync<string>("SetLocalStorage", key, item);
            }
            catch (Exception ex)
            {
                
            }
        }

        public async Task DeleteLocalStorage(string key)
        {
            try
            {
                var module = await moduleTask.Value;
                await module.InvokeAsync<string>("DeleteLocalStorage", key);
            }
            catch (Exception ex)
            {

            }
        }

        public async Task DeleteAllLocalStorage()
        {
            try
            {
                var module = await moduleTask.Value;
                await module.InvokeAsync<string>("DeleteAllLocalStorage");
            }
            catch (Exception ex)
            {

            }
        }

        public async ValueTask<string> GetSessionStorage(string key)
        {
            try
            {
                var module = await moduleTask.Value;
                return await module.InvokeAsync<string>("GetSessionStorage", key);
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public async Task SetSessionStorage(string key, string item)
        {
            try
            {
                var module = await moduleTask.Value;
                await module.InvokeAsync<string>("SetSessionStorage", key, item);
            }
            catch (Exception ex)
            {

            }
        }

        public async Task DeleteSessionStorage(string key)
        {
            try
            {
                var module = await moduleTask.Value;
                await module.InvokeAsync<string>("DeleteSessionStorage", key);
            }
            catch (Exception ex)
            {

            }
        }

        public async Task DeleteAllSessionStorage()
        {
            try
            {
                var module = await moduleTask.Value;
                await module.InvokeAsync<string>("DeleteAllSessionStorage");
            }
            catch (Exception ex)
            {

            }
        }

        public async void SubscribeToStorageEvent(DotNetObjectReference<IStateService> objRef)
        {
            try
            {
                var module = await moduleTask.Value;
                await module.InvokeVoidAsync("SubscribeToStorageEvent", objRef);
            }
            catch (Exception ex)
            {

            }
        }

        public async ValueTask DisposeAsync()
        {
            if (moduleTask.IsValueCreated)
            {
                var module = await moduleTask.Value;
                await module.DisposeAsync();
            }
        }
    }
}