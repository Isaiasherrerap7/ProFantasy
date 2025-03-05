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
    // EditContext se usa para manejar la validaci�n del formulario
    private EditContext editContext = null!;

    // Pa�s seleccionado en el formulario
    private Country selectedCountry = new();

    // Lista de pa�ses obtenidos desde la API
    private List<Country>? countries;

    // URL de la imagen del equipo (si ya existe)
    private string? imageUrl;

    // Mensaje informativo sobre la forma de la imagen (cuadrada o rectangular)
    private string? shapeImageMessage;

    // Inyecci�n de dependencias necesarias para el componente
    [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;

    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;

    // Modelo del formulario (TeamDTO)
    [EditorRequired, Parameter] public TeamDTO TeamDTO { get; set; } = null!;

    // Evento que se ejecuta cuando el formulario se env�a correctamente
    [EditorRequired, Parameter] public EventCallback OnValidSubmit { get; set; }

    // Evento para manejar la acci�n de retorno (por ejemplo, cerrar el formulario)
    [EditorRequired, Parameter] public EventCallback ReturnAction { get; set; }

    // Indica si el formulario se envi� correctamente
    public bool FormPostedSuccessfully { get; set; } = false;

    /// <summary>
    /// Inicializa el EditContext con el modelo `TeamDTO`
    /// </summary>
    protected override void OnInitialized()
    {
        editContext = new(TeamDTO);
    }

    /// <summary>
    /// Se ejecuta de manera as�ncrona al inicializar el componente y carga los pa�ses disponibles.
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        await LoadCountriesAsync();
    }

    /// <summary>
    /// Se ejecuta cuando los par�metros del componente cambian.
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

        // Establece el mensaje de informaci�n sobre la forma de la imagen
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
    /// Obtiene la lista de pa�ses desde la API para mostrarlos en un ComboBox.
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

        // Almacena los pa�ses obtenidos
        countries = responseHttp.Response;
    }

    /// <summary>
    /// Se ejecuta cuando el usuario selecciona una nueva imagen.
    /// </summary>
    private void ImageSelected(string imagenBase64)
    {
        // Guarda la imagen en Base64 en el modelo y limpia la imagen previa si exist�a.
        TeamDTO.Image = imagenBase64;
        imageUrl = null;
    }

    /// <summary>
    /// Maneja la confirmaci�n antes de salir de la p�gina si el formulario ha sido editado.
    /// </summary>
    private async Task OnBeforeInternalNavigation(LocationChangingContext context)
    {
        var formWasEdited = editContext.IsModified();

        // Si el formulario no fue modificado o ya se envi� correctamente, se permite la navegaci�n
        if (!formWasEdited || FormPostedSuccessfully)
        {
            return;
        }

        // Muestra un cuadro de confirmaci�n para evitar perder cambios
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
            return;  // Permite la navegaci�n
        }

        // Previene la navegaci�n si el usuario cancela la acci�n
        context.PreventNavigation();
    }

    /// <summary>
    /// Filtra la lista de pa�ses en funci�n del texto ingresado por el usuario en el buscador.
    /// </summary>
    private async Task<IEnumerable<Country>> SearchCountry(string searchText, CancellationToken cancellationToken)
    {
        await Task.Delay(5); // Simula una peque�a espera para mejorar la experiencia de b�squeda

        if (string.IsNullOrWhiteSpace(searchText))
        {
            return countries!; // Retorna todos los pa�ses si no hay b�squeda
        }

        return countries!
            .Where(x => x.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    /// <summary>
    /// Se ejecuta cuando el usuario selecciona un pa�s en el formulario.
    /// </summary>
    private void CountryChanged(Country country)
    {
        selectedCountry = country;
        TeamDTO.CountryId = country.Id; // Asigna el ID del pa�s seleccionado al equipo
    }
}