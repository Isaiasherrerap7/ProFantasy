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

    // Inyecci�n del servicio de repositorio para realizar solicitudes HTTP al backend.
    [Inject] private IRepository Repository { get; set; } = null!;

    // Inyecci�n del servicio de navegaci�n para redirigir a otras p�ginas.
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    // Inyecci�n del servicio de notificaciones tipo Snackbar.
    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    // Inyecci�n del servicio de localizaci�n para manejar textos en diferentes idiomas.
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

    // M�todo para crear un nuevo pa�s en el backend.
    private async Task CreateAsync()
    {
        // Env�a una solicitud POST al backend para crear el pa�s.
        var responseHttp = await Repository.PostAsync("/api/countries", country);

        // Si la respuesta contiene un error, muestra un mensaje en el Snackbar.
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message], Severity.Error);
            return;
        }

        // Si la solicitud es exitosa, redirige a la lista de pa�ses.
        Return();

        // Muestra una notificaci�n de �xito.
        Snackbar.Add(Localizer["RecordCreatedOk"], Severity.Success);
    }

    // M�todo para regresar a la lista de pa�ses.
    private void Return()
    {
        // Indica que el formulario se envi� correctamente.
        countryForm!.FormPostedSuccessfully = true;

        // Redirige a la p�gina de pa�ses.
        NavigationManager.NavigateTo("/countries");
    }
}