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
    // Contexto de edici�n para manejar la validaci�n y el estado del formulario.
    private EditContext editContext = null!;

    // M�todo que se ejecuta cuando el componente se inicializa.
    protected override void OnInitialized()
    {
        // Inicializa el EditContext con el objeto Country.
        editContext = new(Country);
    }

    // Propiedad obligatoria que representa el objeto Country (pa�s) que se va a crear o editar.
    [EditorRequired, Parameter] public Country Country { get; set; } = null!;

    // Evento obligatorio que se invoca cuando el formulario se env�a correctamente.
    [EditorRequired, Parameter] public EventCallback OnValidSubmit { get; set; }

    // Evento obligatorio que se invoca cuando el usuario decide regresar sin guardar cambios.
    [EditorRequired, Parameter] public EventCallback ReturnAction { get; set; }

    // Indica si el formulario se envi� correctamente.
    public bool FormPostedSuccessfully { get; set; } = false;

    // Servicio de SweetAlert2 para mostrar alertas y mensajes.
    [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;

    // Servicio de localizaci�n para manejar textos en diferentes idiomas.
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

    // M�todo que se ejecuta antes de que el usuario navegue fuera del formulario.
    private async Task OnBeforeInternalNavigation(LocationChangingContext context)
    {
        // Verifica si el formulario ha sido modificado.
        var formWasEdited = editContext.IsModified();

        // Si el formulario no ha sido modificado o ya se envi� correctamente, permite la navegaci�n.
        if (!formWasEdited || FormPostedSuccessfully)
        {
            return;
        }

        // Muestra una alerta de confirmaci�n si el usuario intenta salir sin guardar cambios.
        var result = await SweetAlertService.FireAsync(new SweetAlertOptions
        {
            Title = Localizer["Confirmation"], // T�tulo de la alerta.
            Text = Localizer["LeaveAndLoseChanges"], // Mensaje de la alerta.
            Icon = SweetAlertIcon.Warning, // �cono de advertencia.
            ShowCancelButton = true // Muestra un bot�n para cancelar.
        });

        // Verifica si el usuario confirm� la acci�n.
        var confirm = !string.IsNullOrEmpty(result.Value);
        if (confirm)
        {
            return; // Permite la navegaci�n.
        }

        // Si el usuario no confirma, previene la navegaci�n.
        context.PreventNavigation();
    }
}