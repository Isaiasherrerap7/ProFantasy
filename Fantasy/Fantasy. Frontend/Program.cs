using Fantasy._Frontend;
using Fantasy._Frontend.Repositories;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
// Conexion con el backend
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7231") });
builder.Services.AddScoped<IRepository, Repository>();
await builder.Build().RunAsync();