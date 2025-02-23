using Fantasy.Shared.Resources;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Fantasy.Frontend.Shared
{
    // Componente genérico que permite al usuario seleccionar una imagen desde su dispositivo.
    public partial class InputImg
    {
        // Almacena la imagen en formato base64.
        private string? imageBase64;

        // Almacena el nombre del archivo seleccionado.
        private string? fileName;

        // Inyecta el servicio de localización para manejar traducciones.
        [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

        // Etiqueta opcional para el input (por defecto se mostrará "Image" si no se establece).
        [Parameter] public string? Label { get; set; }

        // URL opcional para mostrar una imagen previa antes de la selección.
        [Parameter] public string? ImageURL { get; set; }

        // Evento que notifica al componente padre cuando se selecciona una imagen, enviando su contenido en base64.
        [Parameter] public EventCallback<string> ImageSelected { get; set; }

        // Se ejecuta cada vez que cambian los parámetros. Asigna la etiqueta por defecto si está vacía o nula.
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (string.IsNullOrWhiteSpace(Label))
            {
                Label = Localizer["Image"]; // Usa la localización para obtener la etiqueta predeterminada.
            }
        }

        // Maneja el evento de cambio cuando el usuario selecciona un nuevo archivo de imagen.
        private async Task OnChange(InputFileChangeEventArgs e)
        {
            var file = e.File; // Obtiene el archivo seleccionado.
            if (file != null)
            {
                fileName = file.Name; // Guarda el nombre del archivo.

                // Crea un arreglo de bytes con el tamaño del archivo y lee su contenido.
                var arrBytes = new byte[file.Size];
                await file.OpenReadStream().ReadAsync(arrBytes);

                // Convierte el contenido del archivo a una cadena en base64.
                imageBase64 = Convert.ToBase64String(arrBytes);

                // Limpia la URL anterior (si existía) al seleccionar una nueva imagen.
                ImageURL = null;

                // Invoca el callback para notificar que hay una nueva imagen disponible en base64.
                await ImageSelected.InvokeAsync(imageBase64);

                // Solicita la actualización de la interfaz tras el cambio.
                StateHasChanged();
            }
        }
    }
}