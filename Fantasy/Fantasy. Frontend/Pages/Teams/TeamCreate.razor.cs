using Fantasy.Frontend.Repositories;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace Fantasy.Frontend.Pages.Teams;

public partial class TeamCreate
{
    // Referencia al formulario del equipo
    private TeamForm? teamForm;

    // Objeto DTO que almacena los datos del nuevo equipo
    private TeamDTO teamDTO = new();

    // Inyección de dependencias necesarias para la funcionalidad del componente
    [Inject] private IRepository Repository { get; set; } = null!;

    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

    /// <summary>
    /// Crea un nuevo equipo enviando los datos a la API.
    /// </summary>
    private async Task CreateAsync()
    {
        // Envia el nuevo equipo a la API
        var responseHttp = await Repository.PostAsync("/api/teams/full", teamDTO);

        // Manejo de errores en caso de fallo en la petición
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }

        // Si la creación es exitosa, regresa a la lista de equipos y muestra una notificación
        Return();
        Snackbar.Add(Localizer["RecordCreatedOk"], Severity.Success);
    }

    /// <summary>
    /// Redirige a la página de equipos después de una operación exitosa.
    /// </summary>
    private void Return()
    {
        // Indica que el formulario se envió correctamente
        teamForm!.FormPostedSuccessfully = true;

        // Redirige a la página de lista de equipos
        NavigationManager.NavigateTo("/teams");
    }
}