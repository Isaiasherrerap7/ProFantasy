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

    // Servicio de navegación para redirigir a otras páginas.
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    // Servicio de SweetAlert2 para mostrar alertas y mensajes.
    [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;

    // Servicio de localización para manejar textos en diferentes idiomas.
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

    // Método para crear un nuevo país.
    private async Task CreateAsync()
    {
        // Envía una solicitud POST al backend para crear el país.
        var responseHttp = await Repository.PostAsync("/api/countries", country);

        // Si hay un error, muestra un mensaje de error.
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            await SweetAlertService.FireAsync(Localizer["Error"], Localizer[message!]);
            return;
        }

        // Si la solicitud es exitosa, redirige a la lista de países.
        Return();

        // Muestra un mensaje de éxito.
        var toast = SweetAlertService.Mixin(new SweetAlertOptions
        {
            Toast = true, // Muestra el mensaje como un toast.
            Position = SweetAlertPosition.BottomEnd, // Posición del toast.
            ShowConfirmButton = true, // Muestra un botón de confirmación.
            Timer = 3000 // Duración del toast.
        });
        await toast.FireAsync(icon: SweetAlertIcon.Success, message: Localizer["RecordCreatedOk"]);
    }

    // Método para regresar a la lista de países.
    private void Return()
    {
        // Indica que el formulario se envió correctamente.
        countryForm!.FormPostedSuccessfully = true;

        // Redirige a la lista de países.
        NavigationManager.NavigateTo("/countries");
    }
}