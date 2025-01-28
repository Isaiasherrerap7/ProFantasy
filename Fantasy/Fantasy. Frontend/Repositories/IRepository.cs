namespace Fantasy._Frontend.Repositories;

public interface IRepository
{
    // Contrato del repositorio para implementar los vervos del controller genericos enbueltos con la clase htttpresponseWrapper
    Task<HttpResponseWrapper<T>> GetAsync<T>(string url);

    Task<HttpResponseWrapper<object>> PostAsync<T>(string url, T model);

    Task<HttpResponseWrapper<TActionResponse>> PostAsync<T, TActionResponse>(string url, T model);
}