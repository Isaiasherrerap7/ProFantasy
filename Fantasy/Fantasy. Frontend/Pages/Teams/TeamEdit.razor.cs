using CurrieTechnologies.Razor.SweetAlert2;
using Fantasy.Frontend.Repositories;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entities;
using Fantasy.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Fantasy.Frontend.Pages.Teams
{
    // Este componente se encarga de la l�gica para editar un equipo existente.
    public partial class TeamEdit
    {
        // Objeto que representar� el equipo a editar.
        private TeamDTO? teamDTO;

        // Referencia al formulario que gestiona la edici�n de un equipo (TeamForm).
        private TeamForm? teamForm;

        // Inyecciones:
        // - NavigationManager: para navegar entre p�ginas.
        // - Repository: para interactuar con la API (obtener, actualizar datos, etc.).
        // - SweetAlertService: para mostrar alertas o notificaciones de manera atractiva.
        // - Localizer: para manejar la localizaci�n de textos (traducciones).
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;

        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

        // Par�metro que indica el Id del equipo a editar, se recibe desde la ruta o el padre.
        [Parameter] public int Id { get; set; }

        // Al inicializar de forma as�ncrona el componente, se realiza una petici�n
        // para obtener la informaci�n del equipo a editar a partir del Id.
        protected override async Task OnInitializedAsync()
        {
            var responseHttp = await Repository.GetAsync<Team>($"api/teams/{Id}");

            // Si hay un error en la respuesta de la API, se maneja seg�n el tipo de error.
            if (responseHttp.Error)
            {
                // Si la respuesta indica que el recurso (equipo) no fue encontrado,
                // se redirige a la lista de equipos.
                if (responseHttp.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    NavigationManager.NavigateTo("teams");
                }
                else
                {
                    // Para otros errores, se muestra un mensaje de alerta al usuario.
                    var messageError = await responseHttp.GetErrorMessageAsync();
                    await SweetAlertService.FireAsync(Localizer["Error"], messageError, SweetAlertIcon.Error);
                }
            }
            else
            {
                // Si no hubo error, se mapea el objeto Team recibido a un TeamDTO
                // para trabajar con �l en el formulario.
                var team = responseHttp.Response;
                teamDTO = new TeamDTO()
                {
                    Id = team!.Id,
                    Name = team!.Name,
                    Image = team.Image,
                    CountryId = team.CountryId
                };
            }
        }

        // M�todo que se encarga de enviar los datos editados al servidor.
        private async Task EditAsync()
        {
            var responseHttp = await Repository.PutAsync("api/teams/full", teamDTO);

            if (responseHttp.Error)
            {
                // Si ocurre un error, se muestra el mensaje correspondiente.
                var mensajeError = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync(Localizer["Error"], Localizer[mensajeError!], SweetAlertIcon.Error);
                return;
            }

            // Si la edici�n se realiza con �xito, se llama a Return para
            // regresar a la lista y se muestra una notificaci�n de �xito (toast).
            Return();
            var toast = SweetAlertService.Mixin(new SweetAlertOptions
            {
                Toast = true,
                Position = SweetAlertPosition.BottomEnd,
                ShowConfirmButton = true,
                Timer = 3000
            });
            await toast.FireAsync(icon: SweetAlertIcon.Success, message: Localizer["RecordSavedOk"]);
        }

        // M�todo que indica que el formulario se proces� correctamente y redirige a la lista de equipos.
        private void Return()
        {
            teamForm!.FormPostedSuccessfully = true;
            NavigationManager.NavigateTo("teams");
        }
    }
}