using Fantasy.Backend.Helpers;
using Fantasy.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fantasy.Backend.Data
{
    // Clase que se encarga de inicializar y "sembrar" la base de datos con datos básicos (paises y equipos).
    public class SeedDb
    {
        // DataContext para acceder y manipular la base de datos a través de Entity Framework.
        private readonly DataContext _context;

        // Servicio de almacenamiento de archivos (IFileStorage),
        // usado para subir imágenes y obtener sus rutas de acceso.
        private readonly IFileStorage _fileStorage;

        // El constructor inyecta el DataContext y el servicio de almacenamiento de archivos.
        public SeedDb(DataContext context, IFileStorage fileStorage)
        {
            _context = context;
            _fileStorage = fileStorage;
        }

        // Método principal para asegurar la creación de la base de datos y
        // para invocar la carga de países y equipos si no existen.
        public async Task SeedAsync()
        {
            // Verifica si la base de datos existe; de lo contrario, la crea y aplica migraciones mínimas.
            await _context.Database.EnsureCreatedAsync();

            // Revisa y carga países si no existen.
            await CheckCountriesAsync();

            // Revisa y carga equipos si no existen.
            await CheckTeamsAsync();
        }

        // Verifica si hay países en la tabla Countries. Si no hay,
        // lee un archivo SQL que contiene las sentencias para insertarlos.
        private async Task CheckCountriesAsync()
        {
            if (!_context.Countries.Any())
            {
                // Lee el script con los INSERT de países.
                var countriesSQLScript = File.ReadAllText("Data\\Countries.sql");
                // Ejecuta ese script en la base de datos, creando los registros de paises.
                await _context.Database.ExecuteSqlRawAsync(countriesSQLScript);
            }
        }

        // Verifica si existen equipos en la tabla Teams. Si no hay,
        // crea uno por cada país y, si está la imagen del país, la almacena
        // mediante el servicio IFileStorage y asocia la ruta resultante.
        private async Task CheckTeamsAsync()
        {
            if (!_context.Teams.Any())
            {
                // Recorre la lista de paises para crear equipos asociados
                foreach (var country in _context.Countries)
                {
                    var imagePath = string.Empty;
                    // Ruta local donde se encuentran las banderas (Images\Flags\{country.Name}.png)
                    var filePath = $"{Environment.CurrentDirectory}\\Images\\Flags\\{country.Name}.png";

                    // Verifica si el archivo existe en la ruta especificada
                    if (File.Exists(filePath))
                    {
                        // Lee la imagen en un array de bytes
                        var fileBytes = File.ReadAllBytes(filePath);
                        // Utiliza el servicio de almacenamiento para guardar la imagen,
                        // obteniendo la ruta final donde quedó almacenada (imagePath).
                        imagePath = await _fileStorage.SaveFileAsync(fileBytes, "jpg", "teams");
                    }

                    // Crea un nuevo registro Team, asignándole el país y la ruta de imagen (si existe).
                    _context.Teams.Add(new Team
                    {
                        Name = country.Name,
                        Country = country!,
                        Image = imagePath
                    });
                }

                // Guarda todos los registros de equipos creados en la base de datos.
                await _context.SaveChangesAsync();
            }
        }
    }
}