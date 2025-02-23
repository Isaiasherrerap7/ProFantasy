using CurrieTechnologies.Razor.SweetAlert2;
using Fantasy.Frontend.Repositories;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Fantasy.Frontend.Pages.Teams
{
    // Componente de página que se encarga de crear un nuevo equipo (Team).
    public partial class TeamCreate
    {
        // Referencia al formulario TeamForm para poder actualizar su estado
        // (por ejemplo, indicar que el formulario se publicó con éxito).
        private TeamForm? teamForm;

        // Objeto que contendrá los datos del equipo a crear.
        private TeamDTO teamDTO = new();

        // Inyecciones de dependencias:
        // - Repository: Interfaz para realizar peticiones HTTP a la API (crear, leer, etc.).
        // - NavigationManager: Permite navegar programáticamente entre páginas.
        // - SweetAlertService: Servicio para mostrar alertas o notificaciones.
        // - Localizer: Servicio de localización para manejar textos traducidos en la interfaz.
        [Inject] private IRepository Repository { get; set; } = null!;

        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

        // Método que se encarga de la creación asincrónica de un nuevo equipo en la API
        // a través del repositorio. Si ocurre un error, muestra un mensaje de alerta,
        // de lo contrario, redirige y muestra un "toast" de confirmación.
        private async Task CreateAsync()
        {
            var responseHttp = await Repository.PostAsync("/api/teams/full", teamDTO);

            if (responseHttp.Error)
            {
                // Obtiene y muestra el mensaje de error retornado por la API.
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync(Localizer["Error"], Localizer[message!], SweetAlertIcon.Error);
                return;
            }

            // Una vez creado el equipo correctamente, se llama al método Return.
            Return();

            // Muestra un "toast" de éxito para notificar que se creó el registro.
            var toast = SweetAlertService.Mixin(new SweetAlertOptions
            {
                Toast = true,
                Position = SweetAlertPosition.BottomEnd,
                ShowConfirmButton = true,
                Timer = 3000
            });
            toast.FireAsync(icon: SweetAlertIcon.Success, message: Localizer["RecordCreatedOk"]);
        }

        // Método que indica que el formulario se procesó exitosamente y redirige a la lista de equipos.
        private void Return()
        {
            // Marca el formulario como exitosamente enviado.
            teamForm!.FormPostedSuccessfully = true;
            // Navega a la página que muestra la lista de equipos.
            NavigationManager.NavigateTo("/teams");
        }
    }
}