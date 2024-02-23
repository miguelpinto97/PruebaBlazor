using Blazored.LocalStorage;
using RestauranteVirtual.Web;
using RestauranteVirtual.Web.Models;
using RestauranteVirtual.Web.Services.API;
using RestauranteVirtual.Web.Services.Authentication;
using RestauranteVirtual.Web.Services.Javascript;
using RestauranteVirtual.Web.Services.Spinner;
using RestauranteVirtual.Web.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

var http = new HttpClient()
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress),
};

Config response = new Config();
string RutaAPI = "";

try
{
	response = await http.GetFromJsonAsync<Config>("config.json");
	RutaAPI = response.API;
}
catch (JsonException ex)
{
	// Manejar el error de deserialización aquí
	Console.WriteLine($"Error de deserialización JSON: {ex.Message}");
}



builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

/* Spinner */
builder.Services.AddScoped<SpinnerService>();
builder.Services.AddScoped<CustomHttpMessageHandler>();
builder.Services.AddScoped(s =>
{
    var accessTokenHandler = s.GetRequiredService<CustomHttpMessageHandler>();
    accessTokenHandler.InnerHandler = new HttpClientHandler();

    return new HttpClient(accessTokenHandler)
    {
        BaseAddress = new Uri(RutaAPI),
        Timeout = TimeSpan.FromMinutes(20)
    };
});

builder.Services.AddAuthorizationCore(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<TokenRenewalService>();

builder.Services.AddTransient<JavascriptService>();


builder.Services.AddTransient<PersonaService>();
builder.Services.AddTransient<AlumnoService>();
builder.Services.AddTransient<ParametroService>();
builder.Services.AddTransient<MenuOpcionesService>(); 
builder.Services.AddTransient<NotificacionesService>();
builder.Services.AddTransient<PerfilService>();
builder.Services.AddTransient<EntidadService>();

builder.Services.AddTransient<PedidoService>();

await builder.Build().RunAsync();
