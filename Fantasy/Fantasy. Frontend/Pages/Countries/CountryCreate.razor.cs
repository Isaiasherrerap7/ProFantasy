using CurrieTechnologies.Razor.SweetAlert2;
using Fantasy.Frontend.Repositories;
using Fantasy.Shared.Entities;
using Fantasy.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace Fantasy.Frontend.Pages.Countries;

public partial class CountryCreate
{
    // Referencia al componente CountryForm.
    private CountryForm? countryForm;

    // Instancia del objeto Country que se va a crear.
    private Country country = new();

    // Inyección del servicio de repositorio para realizar solicitudes HTTP al backend.
    [Inject] private IRepository Repository { get; set; } = null!;

    // Inyección del servicio de navegación para redirigir a otras páginas.
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    // Inyección del servicio de notificaciones tipo Snackbar.
    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    // Inyección del servicio de localización para manejar textos en diferentes idiomas.
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

    // Método para crear un nuevo país en el backend.
    private async Task CreateAsync()
    {
        // Envía una solicitud POST al backend para crear el país.
        var responseHttp = await Repository.PostAsync("/api/countries", country);

        // Si la respuesta contiene un error, muestra un mensaje en el Snackbar.
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message], Severity.Error);
            return;
        }

        // Si la solicitud es exitosa, redirige a la lista de países.
        Return();

        // Muestra una notificación de éxito.
        Snackbar.Add(Localizer["RecordCreatedOk"], Severity.Success);
    }

    // Método para regresar a la lista de países.
    private void Return()
    {
        // Indica que el formulario se envió correctamente.
        countryForm!.FormPostedSuccessfully = true;

        // Redirige a la página de países.
        NavigationManager.NavigateTo("/countries");
    }
}