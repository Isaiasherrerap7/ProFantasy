using CurrieTechnologies.Razor.SweetAlert2;
using Fantasy.Frontend.Repositories;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Fantasy.Frontend.Pages.Teams
{
    // Componente de p�gina que se encarga de crear un nuevo equipo (Team).
    public partial class TeamCreate
    {
        // Referencia al formulario TeamForm para poder actualizar su estado
        // (por ejemplo, indicar que el formulario se public� con �xito).
        private TeamForm? teamForm;

        // Objeto que contendr� los datos del equipo a crear.
        private TeamDTO teamDTO = new();

        // Inyecciones de dependencias:
        // - Repository: Interfaz para realizar peticiones HTTP a la API (crear, leer, etc.).
        // - NavigationManager: Permite navegar program�ticamente entre p�ginas.
        // - SweetAlertService: Servicio para mostrar alertas o notificaciones.
        // - Localizer: Servicio de localizaci�n para manejar textos traducidos en la interfaz.
        [Inject] private IRepository Repository { get; set; } = null!;

        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

        // M�todo que se encarga de la creaci�n asincr�nica de un nuevo equipo en la API
        // a trav�s del repositorio. Si ocurre un error, muestra un mensaje de alerta,
        // de lo contrario, redirige y muestra un "toast" de confirmaci�n.
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

            // Una vez creado el equipo correctamente, se llama al m�todo Return.
            Return();

            // Muestra un "toast" de �xito para notificar que se cre� el registro.
            var toast = SweetAlertService.Mixin(new SweetAlertOptions
            {
                Toast = true,
                Position = SweetAlertPosition.BottomEnd,
                ShowConfirmButton = true,
                Timer = 3000
            });
            toast.FireAsync(icon: SweetAlertIcon.Success, message: Localizer["RecordCreatedOk"]);
        }

        // M�todo que indica que el formulario se proces� exitosamente y redirige a la lista de equipos.
        private void Return()
        {
            // Marca el formulario como exitosamente enviado.
            teamForm!.FormPostedSuccessfully = true;
            // Navega a la p�gina que muestra la lista de equipos.
            NavigationManager.NavigateTo("/teams");
        }
    }
}