using BlazorEssentials.Models;
using Microsoft.JSInterop;

namespace BlazorEssentials.Test.Models
{
    public class StateManager : StateService
    {
        public StateManager(IJSRuntime js) : base(js)
        {
        }

        public GeoLocation Location
        {
            get { return GetStateItem<GeoLocation>("GeoLocation"); }
            set { SetStateItem("GeoLocation", value); }
        }

        public GeoLocation PersistentLocation
        {
            get { return GetPersistentStateItem<GeoLocation>("GeoLocation"); }
            set { SetPersistentStateItem("GeoLocation", value); }
        }
    }
}
