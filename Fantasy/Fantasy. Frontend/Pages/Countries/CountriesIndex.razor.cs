using Fantasy.Frontend.Repositories;
using Fantasy.Shared.Entities;
using Fantasy.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Fantasy.Frontend.Pages.Countries;

public partial class CountriesIndex

{
    // Inyecta para acceder al repositorio al protocolo http para acceder a los datos del backend
    [Inject] private IRepository Repository { get; set; } = null!;

    // Inyecta para acceder a los literales de los nombre de titulos en idioma ingles y español
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

    // Prop lista de country puede ser nulo puede no a ver datos
    private List<Country>? Countries { get; set; }

    // cuando cargue la pagina cargue los paises
    protected override async Task OnInitializedAsync()
    {
        // traer la lista de paises desde el controlador
        var responseHppt = await Repository.GetAsync<List<Country>>("api/countries");
        Countries = responseHppt.Response!;
    }
}