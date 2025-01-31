namespace Fantasy.Frontend.Repositories;

public interface IRepository
{
    // Contrato del repositorio para implementar los vervos del controller genericos enbueltos con la clase htttpresponseWrapper

    // Eliminar un recurso
    Task<HttpResponseWrapper<object>> DeleteAsync(string url);

    // Actualizar un recurso
    Task<HttpResponseWrapper<object>> PutAsync<T>(string url, T model);

    // Actualizar y recibir respuesta
    Task<HttpResponseWrapper<TActionResponse>> PutAsync<T, TActionResponse>(string url, T model);

    // Obtener datos
    Task<HttpResponseWrapper<T>> GetAsync<T>(string url);

    //Crear un recurso
    Task<HttpResponseWrapper<object>> PostAsync<T>(string url, T model);

    //Crear y recibir respuesta
    Task<HttpResponseWrapper<TActionResponse>> PostAsync<T, TActionResponse>(string url, T model);
}