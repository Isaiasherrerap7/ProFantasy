using CurrieTechnologies.Razor.SweetAlert2;
using Fantasy.Frontend.Repositories;
using Fantasy.Shared.Entities;
using Fantasy.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;
using System;
using Fantasy.Frontend.Pages.Countries;
using Fantasy.Frontend.Shared;
using MudBlazor;
using System.Net;

namespace Fantasy.Frontend.Pages.Teams;

// Este componente representa una página que muestra un listado de equipos con paginación y opciones de filtrado.
public partial class TeamsIndex
{
    // Lista de equipos obtenidos desde el backend
    private List<Team>? Teams { get; set; }

    // Tabla de MudBlazor para mostrar los equipos
    private MudTable<Team> table = new();

    // Opciones de tamaño de página para la paginación
    private readonly int[] pageSizeOptions = { 10, 25, 50, int.MaxValue };

    // Total de registros en la base de datos
    private int totalRecords = 0;

    // Bandera que indica si se está cargando información
    private bool loading;

    // URL base para acceder a la API de equipos
    private const string baseUrl = "api/teams";

    // Formato del texto de información de paginación
    private string infoFormat = "{first_item}-{last_item} => {all_items}";

    // Inyección de dependencias en el componente
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    // Parámetro recibido desde la URL para filtrar equipos
    [Parameter, SupplyParameterFromQuery] public string Filter { get; set; } = string.Empty;

    /// <summary>
    /// Se ejecuta cuando el componente se inicializa.
    /// Carga la cantidad total de registros desde la API.
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        await LoadTotalRecordsAsync();
    }

    /// <summary>
    /// Carga el número total de registros de equipos desde la API.
    /// Si hay un filtro activo, lo incluye en la consulta.
    /// </summary>
    private async Task LoadTotalRecordsAsync()
    {
        loading = true;
        var url = $"{baseUrl}/totalRecordsPaginated";

        if (!string.IsNullOrWhiteSpace(Filter))
        {
            url += $"?filter={Filter}";
        }

        // Realiza la petición a la API
        var responseHttp = await Repository.GetAsync<int>(url);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }

        // Almacena la cantidad total de registros
        totalRecords = responseHttp.Response;
        loading = false;
    }

    /// <summary>
    /// Carga la lista de equipos de forma paginada desde la API.
    /// </summary>
    private async Task<TableData<Team>> LoadListAsync(TableState state, CancellationToken cancellationToken)
    {
        int page = state.Page + 1; // MudBlazor usa índices de página basados en 0
        int pageSize = state.PageSize;
        var url = $"{baseUrl}/paginated/?page={page}&recordsnumber={pageSize}";

        if (!string.IsNullOrWhiteSpace(Filter))
        {
            url += $"&filter={Filter}";
        }

        // Obtiene los datos de la API
        var responseHttp = await Repository.GetAsync<List<Team>>(url);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return new TableData<Team> { Items = [], TotalItems = 0 };
        }

        if (responseHttp.Response == null)
        {
            return new TableData<Team> { Items = [], TotalItems = 0 };
        }

        return new TableData<Team>
        {
            Items = responseHttp.Response,
            TotalItems = totalRecords
        };
    }

    /// <summary>
    /// Aplica un filtro a la lista y recarga los datos.
    /// </summary>
    private async Task SetFilterValue(string value)
    {
        Filter = value;
        await LoadTotalRecordsAsync();
        await table.ReloadServerData();
    }

    /// <summary>
    /// Muestra un modal para crear o editar un equipo.
    /// </summary>
    private async Task ShowModalAsync(int id = 0, bool isEdit = false)
    {
        var options = new DialogOptions() { CloseOnEscapeKey = true, CloseButton = true };
        IDialogReference? dialog;

        if (isEdit)
        {
            var parameters = new DialogParameters
            {
                { "Id", id }
            };
            dialog = DialogService.Show<TeamEdit>($"{Localizer["Edit"]} {Localizer["Team"]}", parameters, options);
        }
        else
        {
            dialog = DialogService.Show<TeamCreate>($"{Localizer["New"]} {Localizer["Team"]}", options);
        }

        var result = await dialog.Result;
        if (result!.Canceled)
        {
            await LoadTotalRecordsAsync();
            await table.ReloadServerData();
        }
    }

    /// <summary>
    /// Muestra un cuadro de diálogo de confirmación y elimina un equipo si se confirma la acción.
    /// </summary>
    private async Task DeleteAsync(Team team)
    {
        var parameters = new DialogParameters
        {
            { "Message", string.Format(Localizer["DeleteConfirm"], Localizer["Team"], team.Name) }
        };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };

        // Muestra el cuadro de confirmación
        var dialog = DialogService.Show<ConfirmDialog>(Localizer["Confirmation"], parameters, options);
        var result = await dialog.Result;

        if (result!.Canceled)
        {
            return;
        }

        // Realiza la petición de eliminación
        var responseHttp = await Repository.DeleteAsync($"{baseUrl}/{team.Id}");
        if (responseHttp.Error)
        {
            // Si el equipo no se encuentra, redirige a la página de equipos
            if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                NavigationManager.NavigateTo("/teams");
            }
            else
            {
                var message = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(Localizer[message!], Severity.Error);
            }
            return;
        }

        // Recarga la lista de equipos tras la eliminación
        await LoadTotalRecordsAsync();
        await table.ReloadServerData();
        Snackbar.Add(Localizer["RecordDeletedOk"], Severity.Success);
    }
}