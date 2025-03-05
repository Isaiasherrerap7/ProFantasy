using CurrieTechnologies.Razor.SweetAlert2;
using Fantasy.Frontend.Repositories;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entities;
using Fantasy.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.Localization;

namespace Fantasy.Frontend.Pages.Teams;

public partial class TeamForm
{
    // EditContext se usa para manejar la validación del formulario
    private EditContext editContext = null!;

    // País seleccionado en el formulario
    private Country selectedCountry = new();

    // Lista de países obtenidos desde la API
    private List<Country>? countries;

    // URL de la imagen del equipo (si ya existe)
    private string? imageUrl;

    // Mensaje informativo sobre la forma de la imagen (cuadrada o rectangular)
    private string? shapeImageMessage;

    // Inyección de dependencias necesarias para el componente
    [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;

    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;

    // Modelo del formulario (TeamDTO)
    [EditorRequired, Parameter] public TeamDTO TeamDTO { get; set; } = null!;

    // Evento que se ejecuta cuando el formulario se envía correctamente
    [EditorRequired, Parameter] public EventCallback OnValidSubmit { get; set; }

    // Evento para manejar la acción de retorno (por ejemplo, cerrar el formulario)
    [EditorRequired, Parameter] public EventCallback ReturnAction { get; set; }

    // Indica si el formulario se envió correctamente
    public bool FormPostedSuccessfully { get; set; } = false;

    /// <summary>
    /// Inicializa el EditContext con el modelo `TeamDTO`
    /// </summary>
    protected override void OnInitialized()
    {
        editContext = new(TeamDTO);
    }

    /// <summary>
    /// Se ejecuta de manera asíncrona al inicializar el componente y carga los países disponibles.
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        await LoadCountriesAsync();
    }

    /// <summary>
    /// Se ejecuta cuando los parámetros del componente cambian.
    /// </summary>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        // Si el equipo ya tiene una imagen guardada, la almacena en `imageUrl` para mostrarla.
        if (!string.IsNullOrEmpty(TeamDTO.Image))
        {
            imageUrl = TeamDTO.Image;
            TeamDTO.Image = null;  // Se limpia el campo para evitar enviar la misma imagen innecesariamente
        }

        // Establece el mensaje de información sobre la forma de la imagen
        shapeImageMessage = TeamDTO.IsImageSquare ? Localizer["ImageIsSquare"] : Localizer["ImageIsRectangular"];
    }

    /// <summary>
    /// Cambia el estado de si la imagen es cuadrada o no y actualiza el mensaje informativo.
    /// </summary>
    private void OnToggledChanged(bool toggled)
    {
        TeamDTO.IsImageSquare = toggled;
        shapeImageMessage = TeamDTO.IsImageSquare ? Localizer["ImageIsSquare"] : Localizer["ImageIsRectangular"];
    }

    /// <summary>
    /// Obtiene la lista de países desde la API para mostrarlos en un ComboBox.
    /// </summary>
    private async Task LoadCountriesAsync()
    {
        var responseHttp = await Repository.GetAsync<List<Country>>("/api/countries/combo");

        // Si ocurre un error, se muestra una alerta con SweetAlert
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
            return;
        }

        // Almacena los países obtenidos
        countries = responseHttp.Response;
    }

    /// <summary>
    /// Se ejecuta cuando el usuario selecciona una nueva imagen.
    /// </summary>
    private void ImageSelected(string imagenBase64)
    {
        // Guarda la imagen en Base64 en el modelo y limpia la imagen previa si existía.
        TeamDTO.Image = imagenBase64;
        imageUrl = null;
    }

    /// <summary>
    /// Maneja la confirmación antes de salir de la página si el formulario ha sido editado.
    /// </summary>
    private async Task OnBeforeInternalNavigation(LocationChangingContext context)
    {
        var formWasEdited = editContext.IsModified();

        // Si el formulario no fue modificado o ya se envió correctamente, se permite la navegación
        if (!formWasEdited || FormPostedSuccessfully)
        {
            return;
        }

        // Muestra un cuadro de confirmación para evitar perder cambios
        var result = await SweetAlertService.FireAsync(new SweetAlertOptions
        {
            Title = Localizer["Confirmation"],
            Text = Localizer["LeaveAndLoseChanges"],
            Icon = SweetAlertIcon.Warning,
            ShowCancelButton = true,
            CancelButtonText = Localizer["Cancel"],
        });

        var confirm = !string.IsNullOrEmpty(result.Value);
        if (confirm)
        {
            return;  // Permite la navegación
        }

        // Previene la navegación si el usuario cancela la acción
        context.PreventNavigation();
    }

    /// <summary>
    /// Filtra la lista de países en función del texto ingresado por el usuario en el buscador.
    /// </summary>
    private async Task<IEnumerable<Country>> SearchCountry(string searchText, CancellationToken cancellationToken)
    {
        await Task.Delay(5); // Simula una pequeña espera para mejorar la experiencia de búsqueda

        if (string.IsNullOrWhiteSpace(searchText))
        {
            return countries!; // Retorna todos los países si no hay búsqueda
        }

        return countries!
            .Where(x => x.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    /// <summary>
    /// Se ejecuta cuando el usuario selecciona un país en el formulario.
    /// </summary>
    private void CountryChanged(Country country)
    {
        selectedCountry = country;
        TeamDTO.CountryId = country.Id; // Asigna el ID del país seleccionado al equipo
    }
}