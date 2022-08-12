using BlazorEssentials.Models;
using Microsoft.JSInterop;
using System.Text.Json;
using BlazorEssentials.Extensions;
using BlazorEssentials.Enums;

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
                Console.Out.WriteLine(ex.Message);
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
                Console.Out.WriteLine(ex.Message);
                return "";
            }
        }

        public async Task<bool> ConfirmAsync(PromptModel prompt)
        {
            try
            {
                var module = await moduleTask.Value;
                return await module.InvokeAsync<bool>("confirmAsync", prompt);
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task ShowModalDialog(string ModalId)
        {
            try
            {
                var module = await moduleTask.Value;
                await module.InvokeVoidAsync("showDialogModal", ModalId);
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
            }
        }

        public async Task CloseModalDialog(string ModalId)
        {
            try
            {
                var module = await moduleTask.Value;
                await module.InvokeVoidAsync("closeDialogModal", ModalId);
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
            }
        }

        public async Task ShowLoader(string Message = "Loading...", LoaderStyle Style = LoaderStyle.Border, LoaderSize Size = LoaderSize.MD, MarkupColor Color = MarkupColor.Primary)
        {
            try
            {
                string cssClasses = $"spinner-{Style.ToCSSString()} spinner-{Style.ToCSSString()}-{Size.ToCSSString()} text-{Color.ToCSSString()}";
                var module = await moduleTask.Value;
                await module.InvokeVoidAsync("showLoader", Message, cssClasses);
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
            }
        }

        public async Task CloseLoader()
        {
            try
            {
                var module = await moduleTask.Value;
                await module.InvokeVoidAsync("closeLoader");
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
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
                Console.Out.WriteLine(ex.Message);
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
                Console.Out.WriteLine(ex.Message);
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
                Console.Out.WriteLine(ex.Message);
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
                Console.Out.WriteLine(ex.Message);
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
                Console.Out.WriteLine(ex.Message);
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
                Console.Out.WriteLine(ex.Message);
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
                Console.Out.WriteLine(ex.Message);
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
                Console.Out.WriteLine(ex.Message);
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

        public async Task<ClientDeviceType> GetDeviceType()
        {
            var module = await moduleTask.Value;
            var mobile = await module.InvokeAsync<bool>("isDevice");

            return mobile ? ClientDeviceType.Mobile : ClientDeviceType.Desktop;
        }

        public async Task<ClientUserAgent> GetUserAgent()
        {
            var module = await moduleTask.Value;
            var response = await module.InvokeAsync<string>("getUserAgent");

            ClientUserAgent ua = new ClientUserAgent();
            return ua.TryParse(response);
        }

        public async void CopyToClipboard(string textToCopy)
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("clipboardCopy", textToCopy);
        }
    }
}