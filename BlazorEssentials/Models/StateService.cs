using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazorEssentials.Models
{
    public interface IStateService
    {
        public T GetStateItem<T>(string key);
        public void SetStateItem<T>(string key, T value);
        public T GetPersistentStateItem<T>(string key);
        public void SetPersistentStateItem<T>(string key, T value);

        public event Action OnChange;
        public event Action OnChangeFromOtherTab;
        public event Action RefreshPage;
    }
    public class StateService : IStateService
    {
        private static string StateKey = "{EC2E66F0-0148-4B75-A2B7-96C2FAD9F200}";
        
        private Dictionary<string, string> PersistentStateItems { get; set; }
        private Dictionary<string, string> SessionStateItems { get; set; }

        

        public event Action OnChange;
        public event Action RefreshPage;
        public event Action OnChangeFromOtherTab;

        private IJSRuntime JS { get; set; }
        private Interop interop { get; set; }


        public StateService(IJSRuntime js, Interop _interop)
        {
            //Console.WriteLine("Got to State Service Constructor...");
            JS = js;
            interop = _interop;
            InitializeState();

            //var dotnetRef = DotNetObjectReference.Create(this);
        }



        public T GetStateItem<T>(string key)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(SessionStateItems[key]);
            }
            catch (Exception ex)
            {
                return default;
            }
        }

        public async void SetStateItem<T>(string key, T value)
        {
            try
            {
                SessionStateItems[key] = JsonSerializer.Serialize(value);
    
                await interop.SetSessionStorage(StateKey, Util.EncodeToBase64(JsonSerializer.Serialize(SessionStateItems)));
                
            }
            catch (Exception ex)
            {

            }
            NotifyStateChange();
        }

        

        private void NotifyStateChange()
        {
            OnChange?.Invoke();
        }

        [JSInvokable]
        public void StorageChanged()
        {
            OnChangeFromOtherTab?.Invoke();
        }

        public async void InitializeState()
        {
            try
            {
                string ExistingPersistentStorage;
                string ExistingSessionStorage;
                
                ExistingPersistentStorage = await interop.GetLocalStorage(StateKey);
                ExistingSessionStorage = await interop.GetSessionStorage(StateKey);
                
                if (!String.IsNullOrEmpty(ExistingPersistentStorage))
                {
                    PersistentStateItems = JsonSerializer.Deserialize<Dictionary<string, string>>(Util.DecodeFromBase64(ExistingPersistentStorage));
                }
                else
                {
                    PersistentStateItems = new();
                }

                if (!String.IsNullOrEmpty(ExistingSessionStorage))
                {
                    SessionStateItems = JsonSerializer.Deserialize<Dictionary<string, string>>(Util.DecodeFromBase64(ExistingSessionStorage));
                }
                else
                {
                    SessionStateItems = new();
                }
            }
            catch (Exception ex)
            {
                PersistentStateItems = new();
                SessionStateItems = new();
            }

            NotifyStateChange();
        }

        public void TryPageRefresh()
        {
            RefreshPage?.Invoke();
            NotifyStateChange();
        }

        public T GetPersistentStateItem<T>(string key)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(PersistentStateItems[key]);
            }
            catch (Exception ex)
            {
                return default;
            }
        }

        public async void SetPersistentStateItem<T>(string key, T value)
        {
            try
            {
                PersistentStateItems[key] = JsonSerializer.Serialize(value);

                await interop.SetLocalStorage(StateKey, Util.EncodeToBase64(JsonSerializer.Serialize(PersistentStateItems)));

            }
            catch (Exception ex)
            {

            }
            NotifyStateChange();
        }

      
    }
}
