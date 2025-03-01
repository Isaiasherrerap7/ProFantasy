using CurrieTechnologies.Razor.SweetAlert2;
using Fantasy.Frontend.Repositories;
using Fantasy.Frontend.Shared;
using Fantasy.Shared.Entities;
using Fantasy.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using System.Net;

namespace Fantasy.Frontend.Pages.Countries;

public partial class CountriesIndex
{
    // Lista de países que se mostrará en la tabla
    private List<Country>? Countries { get; set; }

    // Referencia a la tabla de MudBlazor para manipulación de datos
    private MudTable<Country> table = new();

    // Opciones de tamaño de página para la paginación de la tabla
    private readonly int[] pageSizeOptions = { 10, 25, 50, int.MaxValue };

    // Variable para almacenar el número total de registros
    private int totalRecords = 0;

    // Indica si se están cargando los datos
    private bool loading;

    // URL base para las llamadas a la API
    private const string baseUrl = "api/countries";

    // Formato de la información en la paginación
    private string infoFormat = "{first_item}-{last_item} => {all_items}";

    // Inyección de dependencias
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    // Filtro aplicado a la lista de países
    [Parameter, SupplyParameterFromQuery] public string Filter { get; set; } = string.Empty;

    // Se ejecuta cuando el componente se inicializa
    protected override async Task OnInitializedAsync()
    {
        await LoadTotalRecordsAsync();
    }

    // Carga el número total de registros en la base de datos
    private async Task LoadTotalRecordsAsync()
    {
        loading = true;
        var url = $"{baseUrl}/totalRecordsPaginated";

        if (!string.IsNullOrWhiteSpace(Filter))
        {
            url += $"?filter={Filter}";
        }

        var responseHttp = await Repository.GetAsync<int>(url);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message], Severity.Error);
            return;
        }

        totalRecords = responseHttp.Response;
        loading = false;
    }

    // Carga los datos de la tabla de manera paginada
    private async Task<TableData<Country>> LoadListAsync(TableState state, CancellationToken cancellationToken)
    {
        int page = state.Page + 1;
        int pageSize = state.PageSize;
        var url = $"{baseUrl}/paginated/?page={page}&recordsnumber={pageSize}";

        if (!string.IsNullOrWhiteSpace(Filter))
        {
            url += $"&filter={Filter}";
        }

        var responseHttp = await Repository.GetAsync<List<Country>>(url);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message], Severity.Error);
            return new TableData<Country> { Items = [], TotalItems = 0 };
        }
        if (responseHttp.Response == null)
        {
            return new TableData<Country> { Items = [], TotalItems = 0 };
        }
        return new TableData<Country>
        {
            Items = responseHttp.Response,
            TotalItems = totalRecords
        };
    }

    // Aplica un filtro de búsqueda a la tabla
    private async Task SetFilterValue(string value)
    {
        Filter = value;
        await LoadTotalRecordsAsync();
        await table.ReloadServerData();
    }

    // Muestra un modal para crear o editar un país
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
            dialog = DialogService.Show<CountryEdit>($"{Localizer["Edit"]} {Localizer["Country"]}", parameters, options);
        }
        else
        {
            dialog = DialogService.Show<CountryCreate>($"{Localizer["New"]} {Localizer["Country"]}", options);
        }

        var result = await dialog.Result;
        if (result!.Canceled)
        {
            await LoadTotalRecordsAsync();
            await table.ReloadServerData();
        }
    }

    // Método para eliminar un país
    private async Task DeleteAsync(Country country)
    {
        var parameters = new DialogParameters
            {
                { "Message", string.Format(Localizer["DeleteConfirm"], Localizer["Country"], country.Name) }
            };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };
        var dialog = DialogService.Show<ConfirmDialog>(Localizer["Confirmation"], parameters, options);
        var result = await dialog.Result;
        if (result!.Canceled)
        {
            return;
        }

        var responseHttp = await Repository.DeleteAsync($"{baseUrl}/{country.Id}");
        if (responseHttp.Error)
        {
            if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                NavigationManager.NavigateTo("/countries");
            }
            else
            {
                var message = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(Localizer[message], Severity.Error);
            }
            return;
        }
        await LoadTotalRecordsAsync();
        await table.ReloadServerData();
        Snackbar.Add(Localizer["RecordDeletedOk"], Severity.Success);
    }
}