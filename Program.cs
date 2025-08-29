using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<PomodoroTimer.App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddSingleton<TimerService>();
builder.Services.AddScoped<LocalStorageService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<ConfigStorageService>();

await builder.Build().RunAsync();
