using Fantasy.Frontend.Repositories;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entities;
using Fantasy.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace Fantasy.Frontend.Pages.Teams;

public partial class TeamEdit
{
    // Objeto DTO que almacena los datos del equipo a editar
    private TeamDTO? teamDTO;

    // Referencia al formulario del equipo
    private TeamForm? teamForm;

    // Pa�s seleccionado para el equipo
    private Country selectedCountry = new();

    // Inyecci�n de dependencias necesarias para el componente
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

    // Par�metro que recibe el ID del equipo a editar
    [Parameter] public int Id { get; set; }

    /// <summary>
    /// Se ejecuta al inicializar el componente y carga los datos del equipo desde la API.
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        // Obtiene los datos del equipo desde la API seg�n el ID proporcionado
        var responseHttp = await Repository.GetAsync<Team>($"api/teams/{Id}");

        // Manejo de errores en caso de que el equipo no exista o haya un problema en la API
        if (responseHttp.Error)
        {
            if (responseHttp.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // Si el equipo no se encuentra, redirige a la lista de equipos
                NavigationManager.NavigateTo("teams");
            }
            else
            {
                // Si hay otro error, muestra una notificaci�n con el mensaje
                var messageError = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(messageError!, Severity.Error);
            }
        }
        else
        {
            // Si la petici�n fue exitosa, almacena los datos en `teamDTO`
            var team = responseHttp.Response;
            teamDTO = new TeamDTO()
            {
                Id = team!.Id,
                Name = team!.Name,
                Image = team.Image,
                CountryId = team.CountryId
            };

            // Guarda el pa�s seleccionado en la variable `selectedCountry`
            selectedCountry = team.Country!;
        }
    }

    /// <summary>
    /// Env�a los cambios del equipo a la API para actualizar sus datos.
    /// </summary>
    private async Task EditAsync()
    {
        // Realiza la petici�n para actualizar el equipo en la API
        var responseHttp = await Repository.PutAsync("api/teams/full", teamDTO);

        // Si hay un error, lo muestra en una notificaci�n
        if (responseHttp.Error)
        {
            var mensajeError = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[mensajeError!], Severity.Error);
            return;
        }

        // Si la actualizaci�n es exitosa, regresa a la lista de equipos y muestra una notificaci�n
        Return();
        Snackbar.Add(Localizer["RecordSavedOk"], Severity.Success);
    }

    /// <summary>
    /// Redirige a la p�gina de equipos despu�s de una operaci�n exitosa.
    /// </summary>
    private void Return()
    {
        // Indica que el formulario se envi� correctamente
        teamForm!.FormPostedSuccessfully = true;

        // Redirige a la p�gina de lista de equipos
        NavigationManager.NavigateTo("teams");
    }
}