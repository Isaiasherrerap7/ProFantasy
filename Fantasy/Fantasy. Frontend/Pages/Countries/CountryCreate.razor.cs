using CurrieTechnologies.Razor.SweetAlert2;
using Fantasy.Frontend.Repositories;
using Fantasy.Shared.Entities;
using Fantasy.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Fantasy.Frontend.Pages.Countries;

public partial class CountryCreate
{
    // Referencia al componente CountryForm.
    private CountryForm? countryForm;

    // Instancia del objeto Country que se va a crear.
    private Country country = new();

    // Servicio de repositorio para realizar solicitudes HTTP al backend.
    [Inject] private IRepository Repository { get; set; } = null!;

    // Servicio de navegaci�n para redirigir a otras p�ginas.
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    // Servicio de SweetAlert2 para mostrar alertas y mensajes.
    [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;

    // Servicio de localizaci�n para manejar textos en diferentes idiomas.
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

    // M�todo para crear un nuevo pa�s.
    private async Task CreateAsync()
    {
        // Env�a una solicitud POST al backend para crear el pa�s.
        var responseHttp = await Repository.PostAsync("/api/countries", country);

        // Si hay un error, muestra un mensaje de error.
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            await SweetAlertService.FireAsync(Localizer["Error"], Localizer[message!]);
            return;
        }

        // Si la solicitud es exitosa, redirige a la lista de pa�ses.
        Return();

        // Muestra un mensaje de �xito.
        var toast = SweetAlertService.Mixin(new SweetAlertOptions
        {
            Toast = true, // Muestra el mensaje como un toast.
            Position = SweetAlertPosition.BottomEnd, // Posici�n del toast.
            ShowConfirmButton = true, // Muestra un bot�n de confirmaci�n.
            Timer = 3000 // Duraci�n del toast.
        });
        await toast.FireAsync(icon: SweetAlertIcon.Success, message: Localizer["RecordCreatedOk"]);
    }

    // M�todo para regresar a la lista de pa�ses.
    private void Return()
    {
        // Indica que el formulario se envi� correctamente.
        countryForm!.FormPostedSuccessfully = true;

        // Redirige a la lista de pa�ses.
        NavigationManager.NavigateTo("/countries");
    }
}