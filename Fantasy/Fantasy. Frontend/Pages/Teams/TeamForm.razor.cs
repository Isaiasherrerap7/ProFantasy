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
    // Componente para manejar el formulario de creación/edición de un equipo.
    public partial class TeamForm
    {
        // Se encarga de hacer el seguimiento de los cambios en el formulario (validaciones, estado sucio, etc.)
        private EditContext editContext = null!;

        // Se inicializa el EditContext con el TeamDTO tan pronto como se crea el componente.
        protected override void OnInitialized()
        {
            editContext = new(TeamDTO);
        }

        // Recibe como parámetro la información del equipo.
        [EditorRequired, Parameter] public TeamDTO TeamDTO { get; set; } = null!;

        // Evento que se ejecuta cuando el formulario pasa la validación y se confirma.
        [EditorRequired, Parameter] public EventCallback OnValidSubmit { get; set; }

        // Evento que se puede invocar para regresar a la vista anterior o cancelar la acción.
        [EditorRequired, Parameter] public EventCallback ReturnAction { get; set; }

        // Indica si el formulario se envió correctamente, para usarlo al gestionar navegación y cambios.
        public bool FormPostedSuccessfully { get; set; } = false;

        // Servicios inyectados:
        // - Para mostrar alertas (SweetAlertService).
        // - Para localización de literales.
        // - Para realizar peticiones al repositorio (API).
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;

        [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;

        // Lista que almacenará los países obtenidos desde la API (para el combobox, por ejemplo).
        private List<Country>? countries;

        // Almacena temporalmente la URL de la imagen, en caso de existir.
        private string? imageUrl;

        // Al inicializar el componente de forma asíncrona, se cargan los países.
        protected override async Task OnInitializedAsync()
        {
            await LoadCountriesAsync();
        }

        // Método que se ejecuta cuando se modifican los parámetros del componente.
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

        // Carga la lista de países consumiendo la API a través del repositorio.
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

        // Maneja la lógica para detectar si el usuario intenta navegar fuera de la página
        // cuando hay cambios sin guardar. Utiliza un cuadro de diálogo de SweetAlertService para confirmar.
        private async Task OnBeforeInternalNavigation(LocationChangingContext context)
        {
            // Revisa si el formulario ha sido modificado y si aún no se envió con éxito.
            var formWasEdited = editContext.IsModified();
            if (!formWasEdited || FormPostedSuccessfully)
            {
                return;
            }

            // Muestra un cuadro de diálogo para confirmar la salida sin guardar cambios.
            var result = await SweetAlertService.FireAsync(new SweetAlertOptions
            {
                Title = Localizer["Confirmation"],
                Text = Localizer["LeaveAndLoseChanges"],
                Icon = SweetAlertIcon.Warning,
                ShowCancelButton = true,
                CancelButtonText = Localizer["Cancel"],
            });

            // Si el usuario elige cancelar (no confirma salir), se impide la navegación.
            var confirm = !string.IsNullOrEmpty(result.Value);
            if (confirm)
            {
                return;
            }

            context.PreventNavigation();
        }
    }
}