using MicrofluidSimulator;
using MicrofluidSimulator.Services.Websocket;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddSingleton<IJSInProcessRuntime>(services => (IJSInProcessRuntime)services.GetRequiredService<IJSRuntime>());
builder.Services.AddSingleton(serviceProvider => (IJSUnmarshalledRuntime)serviceProvider.GetRequiredService<IJSRuntime>());
builder.Services.AddSingleton<WebSocketService>();

await builder.Build().RunAsync();
