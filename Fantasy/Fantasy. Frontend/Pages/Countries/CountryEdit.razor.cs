using CurrieTechnologies.Razor.SweetAlert2;
using Fantasy.Frontend.Repositories;
using Fantasy.Shared.Entities;
using Fantasy.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace Fantasy.Frontend.Pages.Countries;

public partial class CountryEdit
{
    // Instancia del objeto Country que se va a editar.
    private Country? country;

    // Referencia al formulario de edición del país.
    private CountryForm? countryForm;

    // Servicio de navegación para redirigir a otras páginas.
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    // Servicio de repositorio para interactuar con la API backend.
    [Inject] private IRepository Repository { get; set; } = null!;

    // Servicio de notificaciones (MudBlazor Snackbar).
    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    // Servicio de localización para manejar múltiples idiomas.
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

    // Parámetro que recibe el ID del país a editar.
    [Parameter] public int Id { get; set; }

    // Método de inicialización que carga los datos del país desde la API.
    protected override async Task OnInitializedAsync()
    {
        // Realiza una solicitud GET para obtener la información del país con el ID dado.
        var responseHttp = await Repository.GetAsync<Country>($"api/countries/{Id}");

        // Manejo de errores en la solicitud.
        if (responseHttp.Error)
        {
            if (responseHttp.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // Si el país no existe, redirige a la lista de países.
                NavigationManager.NavigateTo("countries");
            }
            else
            {
                // Si hay otro tipo de error, muestra una notificación con el mensaje recibido.
                var messageError = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(messageError!, Severity.Error);
            }
        }
        else
        {
            // Si la solicitud es exitosa, asigna la respuesta al objeto `country`.
            country = responseHttp.Response;
        }
    }

    // Método para guardar los cambios del país editado.
    private async Task EditAsync()
    {
        // Envía una solicitud PUT para actualizar el país en el backend.
        var responseHttp = await Repository.PutAsync("api/countries", country);

        // Si hay un error, muestra un mensaje en el Snackbar y detiene la ejecución.
        if (responseHttp.Error)
        {
            var messageError = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(messageError!, Severity.Error);
            return;
        }

        // Si la actualización es exitosa, regresa a la lista de países.
        Return();

        // Muestra una notificación de éxito con el mensaje de confirmación.
        Snackbar.Add(Localizer["RecordSavedOk"], Severity.Success);
    }

    // Método para regresar a la lista de países después de guardar los cambios.
    private void Return()
    {
        // Indica que el formulario se ha enviado correctamente.
        countryForm!.FormPostedSuccessfully = true;

        // Redirige a la página de listado de países.
        NavigationManager.NavigateTo("countries");
    }
}