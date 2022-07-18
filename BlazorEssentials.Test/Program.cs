using BlazorEssentials.Extensions;
using BlazorEssentials.Test;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazorEssentials("https://localhost:5000/api");

await builder.Build().RunAsync();
