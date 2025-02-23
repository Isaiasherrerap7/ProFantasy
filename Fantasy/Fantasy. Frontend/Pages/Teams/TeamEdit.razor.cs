using CurrieTechnologies.Razor.SweetAlert2;
using Fantasy.Frontend.Repositories;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entities;
using Fantasy.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Fantasy.Frontend.Pages.Teams
{
    // Este componente se encarga de la lógica para editar un equipo existente.
    public partial class TeamEdit
    {
        // Objeto que representará el equipo a editar.
        private TeamDTO? teamDTO;

        // Referencia al formulario que gestiona la edición de un equipo (TeamForm).
        private TeamForm? teamForm;

        // Inyecciones:
        // - NavigationManager: para navegar entre páginas.
        // - Repository: para interactuar con la API (obtener, actualizar datos, etc.).
        // - SweetAlertService: para mostrar alertas o notificaciones de manera atractiva.
        // - Localizer: para manejar la localización de textos (traducciones).
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;

        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

        // Parámetro que indica el Id del equipo a editar, se recibe desde la ruta o el padre.
        [Parameter] public int Id { get; set; }

        // Al inicializar de forma asíncrona el componente, se realiza una petición
        // para obtener la información del equipo a editar a partir del Id.
        protected override async Task OnInitializedAsync()
        {
            var responseHttp = await Repository.GetAsync<Team>($"api/teams/{Id}");

            // Si hay un error en la respuesta de la API, se maneja según el tipo de error.
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
                // para trabajar con él en el formulario.
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

        // Método que se encarga de enviar los datos editados al servidor.
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

            // Si la edición se realiza con éxito, se llama a Return para
            // regresar a la lista y se muestra una notificación de éxito (toast).
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

        // Método que indica que el formulario se procesó correctamente y redirige a la lista de equipos.
        private void Return()
        {
            teamForm!.FormPostedSuccessfully = true;
            NavigationManager.NavigateTo("teams");
        }
    }
}