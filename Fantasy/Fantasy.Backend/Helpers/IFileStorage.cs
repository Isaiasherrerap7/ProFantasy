namespace Fantasy.Backend.Helpers
{
    // Interfaz que define la lógica necesaria para manejar el almacenamiento de archivos.
    // El objetivo es desacoplar la lógica de almacenamiento y permitir diferentes implementaciones
    // (por ejemplo, almacenamiento local, en Azure Blob Storage, AWS S3, etc.).

    public interface IFileStorage
    {
        // Método para guardar un archivo:
        // - 'content' son los bytes del archivo.
        // - 'extension' define la extensión (p. ej., "jpg", "png").
        // - 'containerName' es el nombre del contenedor o carpeta donde se guardará.
        // Devuelve la ruta o URL donde se almacenó finalmente el archivo.
        Task<string> SaveFileAsync(byte[] content, string extention, string containerName);

        // Método para eliminar un archivo en el contenedor especificado:
        // - 'path' es la ruta o nombre del archivo a eliminar.
        // - 'containerName' es el contenedor o carpeta que contiene el archivo.
        Task RemoveFileAsync(string path, string containerName);

        // Método para editar un archivo:
        // - Si se provee 'path', primero elimina el archivo existente.
        // - Luego, llama a SaveFileAsync para guardar el nuevo archivo y retornar su nueva ruta.
        async Task<string> EditFileAsync(byte[] content, string extention, string containerName, string path)
        {
            if (path is not null)
            {
                // Elimina el archivo anterior si existe.
                await RemoveFileAsync(path, containerName);
            }

            // Guarda el nuevo contenido y devuelve la ruta donde se almacenó.
            return await SaveFileAsync(content, extention, containerName);
        }
    }
}