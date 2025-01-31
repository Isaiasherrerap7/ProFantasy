using CurrieTechnologies.Razor.SweetAlert2;
using Fantasy.Shared.Entities;
using Fantasy.Shared.Resources;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Fantasy.Frontend.Pages.Countries;

public partial class CountryForm
{
    // Contexto de edición para manejar la validación y el estado del formulario.
    private EditContext editContext = null!;

    // Método que se ejecuta cuando el componente se inicializa.
    protected override void OnInitialized()
    {
        // Inicializa el EditContext con el objeto Country.
        editContext = new(Country);
    }

    // Propiedad obligatoria que representa el objeto Country (país) que se va a crear o editar.
    [EditorRequired, Parameter] public Country Country { get; set; } = null!;

    // Evento obligatorio que se invoca cuando el formulario se envía correctamente.
    [EditorRequired, Parameter] public EventCallback OnValidSubmit { get; set; }

    // Evento obligatorio que se invoca cuando el usuario decide regresar sin guardar cambios.
    [EditorRequired, Parameter] public EventCallback ReturnAction { get; set; }

    // Indica si el formulario se envió correctamente.
    public bool FormPostedSuccessfully { get; set; } = false;

    // Servicio de SweetAlert2 para mostrar alertas y mensajes.
    [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;

    // Servicio de localización para manejar textos en diferentes idiomas.
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

    // Método que se ejecuta antes de que el usuario navegue fuera del formulario.
    private async Task OnBeforeInternalNavigation(LocationChangingContext context)
    {
        // Verifica si el formulario ha sido modificado.
        var formWasEdited = editContext.IsModified();

        // Si el formulario no ha sido modificado o ya se envió correctamente, permite la navegación.
        if (!formWasEdited || FormPostedSuccessfully)
        {
            return;
        }

        // Muestra una alerta de confirmación si el usuario intenta salir sin guardar cambios.
        var result = await SweetAlertService.FireAsync(new SweetAlertOptions
        {
            Title = Localizer["Confirmation"], // Título de la alerta.
            Text = Localizer["LeaveAndLoseChanges"], // Mensaje de la alerta.
            Icon = SweetAlertIcon.Warning, // Ícono de advertencia.
            ShowCancelButton = true // Muestra un botón para cancelar.
        });

        // Verifica si el usuario confirmó la acción.
        var confirm = !string.IsNullOrEmpty(result.Value);
        if (confirm)
        {
            return; // Permite la navegación.
        }

        // Si el usuario no confirma, previene la navegación.
        context.PreventNavigation();
    }
}