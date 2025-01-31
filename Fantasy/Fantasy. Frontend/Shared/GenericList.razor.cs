using Fantasy.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Fantasy.Frontend.Shared
{
    //Titem: Es un parámetro de tipo genérico que representa el tipo de los elementos en la lista.
    public partial class GenericList<Titem>

    {
        // Inyecta para acceder a los literales de los nombre de titulos en idioma ingles y español
        [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

        //Loading: Un RenderFragment opcional que se muestra mientras la lista está cargando.
        [Parameter] public RenderFragment? Loading { get; set; }

        //NoRecords: Un RenderFragment opcional que se muestra si la lista está vacía.
        [Parameter] public RenderFragment? NoRecords { get; set; }

        //Body: Un RenderFragment obligatorio que define cómo se renderiza la lista cuando contiene datos.
        [EditorRequired, Parameter] public RenderFragment Body { get; set; } = null!;

        //MyList: Una lista obligatoria de elementos de tipo Titem que se va a mostrar.
        [EditorRequired, Parameter] public List<Titem> MyList { get; set; } = null!;
    }
}