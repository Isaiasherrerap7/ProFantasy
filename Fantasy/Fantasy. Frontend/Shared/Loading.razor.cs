using Fantasy.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Fantasy.Frontend.Shared;

public partial class Loading
{
    // Servicio de localización para manejar textos en diferentes idiomas.
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

    // Parámetro opcional que define el texto mostrado en el indicador de carga.
    [Parameter] public string? Label { get; set; }

    // Método que se ejecuta cuando los parámetros del componente cambian.
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        // Si no se proporciona un texto personalizado, se usa el valor predeterminado "PleaseWait".
        if (string.IsNullOrEmpty(Label))
        {
            Label = Localizer["PleaseWait"];
        }
    }
}