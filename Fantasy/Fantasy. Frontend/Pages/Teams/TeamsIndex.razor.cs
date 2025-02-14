using CurrieTechnologies.Razor.SweetAlert2;
using Fantasy.Frontend.Repositories;
using Fantasy.Shared.Entities;
using Fantasy.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;
using System;

namespace Fantasy.Frontend.Pages.Teams;

public partial class TeamsIndex
{
    // 1.Inyecta IRepository para acceder al repositorio al protocolo http para acceder a los datos del backend api
    [Inject] private IRepository Repository { get; set; } = null!;

    //2. Inyecta IStringLocalizer para acceder a los literales de los nombre de titulos en idioma ingles y español
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

    //3. Permite controlar la navegación en Blazor
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    //4. Servicio que proporciona notificaciones emergentes personalizables
    [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;

    // 5. prop Almacena la lista de equipos (Team) que se obtiene del backend API.
    private List<Team>? Teams { get; set; }

    // Cargar los Equipos al Inicializar la Página
    // Cuando el componente se renderiza por primera vez (OnInitializedAsync()), llama a LoadAsync(). LoadAsync() obtiene los equipos desde el backend.
    protected override async Task OnInitializedAsync()
    {
        await LoadAsync();
    }

    // Método LoadAsync() (Cargar los Equipos)
    // Llama al backend usando Repository.GetAsync<List<Team>>("api/teams").
    // Si hay un error en la respuesta, muestra una alerta de error(SweetAlertService).
    // Si la respuesta es exitosa, guarda los equipos en Teams.
    private async Task LoadAsync()
    {
        var responseHppt = await Repository.GetAsync<List<Team>>("api/teams");
        if (responseHppt.Error)
        {
            var message = await responseHppt.GetErrorMessageAsync();
            await SweetAlertService.FireAsync(Localizer["Error"], message, SweetAlertIcon.Error);
            return;
        }
        Teams = responseHppt.Response!;
    }

    // Método DeleteAsync(Team team) (Eliminar un Equipo)
    // Muestra un SweetAlert para confirmar la eliminación.
    //Si el usuario presiona "Cancelar", la acción se detiene(return).
    //Si presiona "Sí", se ejecuta la eliminación.

    //Llama a Repository.DeleteAsync($"api/teams/{team.Id}") para eliminar el equipo.
    //Si el equipo no existe (404), redirige al usuario a / usando NavigationManager.
    //Si hay otro error, muestra un mensaje con SweetAlertService.

    //Si la eliminación es exitosa, recarga la lista de equipos (await LoadAsync();).

    //Muestra un Toast con mensaje de éxito.
    //El Toast aparece en la parte inferior derecha y se cierra automáticamente en 3 segundos.

    private async Task DeleteAsync(Team team)
    {
        var result = await SweetAlertService.FireAsync(new SweetAlertOptions
        {
            Title = Localizer["Confirmation"],
            Text = string.Format(Localizer["DeleteConfirm"], Localizer["Team"], team.Name),
            Icon = SweetAlertIcon.Question,
            ShowCancelButton = true,
            CancelButtonText = Localizer["Cancel"]
        });

        var confirm = string.IsNullOrEmpty(result.Value);

        if (confirm)
        {
            return;
        }

        var responseHttp = await Repository.DeleteAsync($"api/teams/{team.Id}");
        if (responseHttp.Error)
        {
            if (responseHttp.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                NavigationManager.NavigateTo("/");
            }
            else
            {
                var mensajeError = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync(Localizer["Error"], mensajeError, SweetAlertIcon.Error);
            }
            return;
        }

        await LoadAsync();
        var toast = SweetAlertService.Mixin(new SweetAlertOptions
        {
            Toast = true,
            Position = SweetAlertPosition.BottomEnd,
            ShowConfirmButton = true,
            Timer = 3000,
            ConfirmButtonText = Localizer["Yes"]
        });
        toast.FireAsync(icon: SweetAlertIcon.Success, message: Localizer["RecordDeletedOk"]);
    }
}