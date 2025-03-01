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

    // Referencia al formulario de edici�n del pa�s.
    private CountryForm? countryForm;

    // Servicio de navegaci�n para redirigir a otras p�ginas.
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    // Servicio de repositorio para interactuar con la API backend.
    [Inject] private IRepository Repository { get; set; } = null!;

    // Servicio de notificaciones (MudBlazor Snackbar).
    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    // Servicio de localizaci�n para manejar m�ltiples idiomas.
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

    // Par�metro que recibe el ID del pa�s a editar.
    [Parameter] public int Id { get; set; }

    // M�todo de inicializaci�n que carga los datos del pa�s desde la API.
    protected override async Task OnInitializedAsync()
    {
        // Realiza una solicitud GET para obtener la informaci�n del pa�s con el ID dado.
        var responseHttp = await Repository.GetAsync<Country>($"api/countries/{Id}");

        // Manejo de errores en la solicitud.
        if (responseHttp.Error)
        {
            if (responseHttp.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // Si el pa�s no existe, redirige a la lista de pa�ses.
                NavigationManager.NavigateTo("countries");
            }
            else
            {
                // Si hay otro tipo de error, muestra una notificaci�n con el mensaje recibido.
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

    // M�todo para guardar los cambios del pa�s editado.
    private async Task EditAsync()
    {
        // Env�a una solicitud PUT para actualizar el pa�s en el backend.
        var responseHttp = await Repository.PutAsync("api/countries", country);

        // Si hay un error, muestra un mensaje en el Snackbar y detiene la ejecuci�n.
        if (responseHttp.Error)
        {
            var messageError = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(messageError!, Severity.Error);
            return;
        }

        // Si la actualizaci�n es exitosa, regresa a la lista de pa�ses.
        Return();

        // Muestra una notificaci�n de �xito con el mensaje de confirmaci�n.
        Snackbar.Add(Localizer["RecordSavedOk"], Severity.Success);
    }

    // M�todo para regresar a la lista de pa�ses despu�s de guardar los cambios.
    private void Return()
    {
        // Indica que el formulario se ha enviado correctamente.
        countryForm!.FormPostedSuccessfully = true;

        // Redirige a la p�gina de listado de pa�ses.
        NavigationManager.NavigateTo("countries");
    }
}