using CurrieTechnologies.Razor.SweetAlert2;
using Fantasy.Frontend.Repositories;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entities;
using Fantasy.Shared.Resources;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Fantasy.Frontend.Pages.Teams
{
    // Componente para manejar el formulario de creaci�n/edici�n de un equipo.
    public partial class TeamForm
    {
        // Se encarga de hacer el seguimiento de los cambios en el formulario (validaciones, estado sucio, etc.)
        private EditContext editContext = null!;

        // Se inicializa el EditContext con el TeamDTO tan pronto como se crea el componente.
        protected override void OnInitialized()
        {
            editContext = new(TeamDTO);
        }

        // Recibe como par�metro la informaci�n del equipo.
        [EditorRequired, Parameter] public TeamDTO TeamDTO { get; set; } = null!;

        // Evento que se ejecuta cuando el formulario pasa la validaci�n y se confirma.
        [EditorRequired, Parameter] public EventCallback OnValidSubmit { get; set; }

        // Evento que se puede invocar para regresar a la vista anterior o cancelar la acci�n.
        [EditorRequired, Parameter] public EventCallback ReturnAction { get; set; }

        // Indica si el formulario se envi� correctamente, para usarlo al gestionar navegaci�n y cambios.
        public bool FormPostedSuccessfully { get; set; } = false;

        // Servicios inyectados:
        // - Para mostrar alertas (SweetAlertService).
        // - Para localizaci�n de literales.
        // - Para realizar peticiones al repositorio (API).
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;

        [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;

        // Lista que almacenar� los pa�ses obtenidos desde la API (para el combobox, por ejemplo).
        private List<Country>? countries;

        // Almacena temporalmente la URL de la imagen, en caso de existir.
        private string? imageUrl;

        // Al inicializar el componente de forma as�ncrona, se cargan los pa�ses.
        protected override async Task OnInitializedAsync()
        {
            await LoadCountriesAsync();
        }

        // M�todo que se ejecuta cuando se modifican los par�metros del componente.
        // Si se ha proporcionado una imagen dentro de TeamDTO, se mueve a la variable imageUrl
        // y se limpia TeamDTO.Image para que no quede duplicada.
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (!string.IsNullOrEmpty(TeamDTO.Image))
            {
                imageUrl = TeamDTO.Image;
                TeamDTO.Image = null;
            }
        }

        // Carga la lista de pa�ses consumiendo la API a trav�s del repositorio.
        // Si ocurre un error en la respuesta, se muestra un mensaje de alerta.
        private async Task LoadCountriesAsync()
        {
            var responseHttp = await Repository.GetAsync<List<Country>>("/api/countries/combo");
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }

            countries = responseHttp.Response;
        }

        // Actualiza la propiedad TeamDTO.Image con la cadena base64 de la imagen seleccionada
        // y limpia la variable imageUrl para evitar conflictos.
        private void ImageSelected(string imagenBase64)
        {
            TeamDTO.Image = imagenBase64;
            imageUrl = null;
        }

        // Maneja la l�gica para detectar si el usuario intenta navegar fuera de la p�gina
        // cuando hay cambios sin guardar. Utiliza un cuadro de di�logo de SweetAlertService para confirmar.
        private async Task OnBeforeInternalNavigation(LocationChangingContext context)
        {
            // Revisa si el formulario ha sido modificado y si a�n no se envi� con �xito.
            var formWasEdited = editContext.IsModified();
            if (!formWasEdited || FormPostedSuccessfully)
            {
                return;
            }

            // Muestra un cuadro de di�logo para confirmar la salida sin guardar cambios.
            var result = await SweetAlertService.FireAsync(new SweetAlertOptions
            {
                Title = Localizer["Confirmation"],
                Text = Localizer["LeaveAndLoseChanges"],
                Icon = SweetAlertIcon.Warning,
                ShowCancelButton = true,
                CancelButtonText = Localizer["Cancel"],
            });

            // Si el usuario elige cancelar (no confirma salir), se impide la navegaci�n.
            var confirm = !string.IsNullOrEmpty(result.Value);
            if (confirm)
            {
                return;
            }

            context.PreventNavigation();
        }
    }
}