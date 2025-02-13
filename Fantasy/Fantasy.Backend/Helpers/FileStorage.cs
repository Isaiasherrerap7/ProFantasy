using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Fantasy.Backend.Helpers;

public class FileStorage : IFileStorage
{
    // 2.atributo de lectura
    private readonly string _connectionString;

    // 1. Constructor para conexion con el storage con el Iconfigurencion
    public FileStorage(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("AzureStorage")!;
    }

    public async Task RemoveFileAsync(string path, string containerName)
    {
        // 5. Nos conectamos al cliente
        var client = new BlobContainerClient(_connectionString, containerName);
        // si el cliente no existe se crea
        await client.CreateIfNotExistsAsync();
        // busque los archivos
        var fileName = Path.GetFileName(path);
        var blob = client.GetBlobClient(fileName);
        // borre si existe
        await blob.DeleteIfExistsAsync();
    }

    public async Task<string> SaveFileAsync(byte[] content, string extention, string containerName)
    {
        // 3. Conectacte al storage (_connectionString)y al nombre de la carpeta containerName (AZURE)
        var client = new BlobContainerClient(_connectionString, containerName);

        // 4. si NO existe creala
        await client.CreateIfNotExistsAsync();
        client.SetAccessPolicy(PublicAccessType.Blob);

        // el nombre del archivo lo conformamos con un (GUID-CODIGO UNICO)
        var fileName = $"{Guid.NewGuid()}{extention}";
        var blob = client.GetBlobClient(fileName);

        using (var ms = new MemoryStream(content))
        {
            await blob.UploadAsync(ms);
        }

        return blob.Uri.ToString();
    }
}