using BlazorEssentials.Extensions;
using BlazorEssentials.Test;
using BlazorEssentials.Test.Models;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazorEssentials("https://localhost:5000/api");
builder.Services.AddScoped<StateManager>();

var host = builder.Build();

var services = host.Services;

var stateMan = services.GetService<StateManager>();
stateMan.InitializeState();

await host.RunAsync();
