using CurrieTechnologies.Razor.SweetAlert2;
using Fantasy.Frontend;
using Fantasy.Frontend.Repositories;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
// Conexion con el backend
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7231") });
builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddLocalization();
builder.Services.AddSweetAlert2();
// Servicio a mudblazor
builder.Services.AddMudServices();

await builder.Build().RunAsync();