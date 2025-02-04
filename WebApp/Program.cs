using Common.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using WebApp;
using WebApp.Security;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Services.AddScoped(sp =>
    new HttpClient
    {
        BaseAddress = new Uri("https://localhost:7263/api/"),
        Timeout = TimeSpan.FromSeconds(30)
    }
);

builder.Services.AddScoped<IWebClient, WebClient>();
builder.Services.AddScoped<ICryptoService, CryptoService>();

builder.Services.AddScoped<JwtAuthenticationProvider>(options => new JwtAuthenticationProvider(options.GetRequiredService<IJSRuntime>()));
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationProvider>();
builder.Services.AddScoped<IJwtSessionService, JwtAuthenticationProvider>(options => options.GetRequiredService<JwtAuthenticationProvider>());
builder.Services.AddAuthorizationCore();

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });


await builder.Build().RunAsync();
