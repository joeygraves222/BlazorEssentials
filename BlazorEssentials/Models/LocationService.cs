using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorEssentials.Models
{
    public interface ILocationService
    {
        Task<GeoLocation> GetLocationAsync();
    }
    public class LocationService : ILocationService
    {
        private Interop interop { get; set; }
        public LocationService(Interop _interop)
        {
            interop = _interop;
        }
        public async Task<GeoLocation> GetLocationAsync()
        {
            return await interop.GetGeoLocation();
        }
    }
}
