using System.Text;
using System.Text.Json;

namespace Fantasy._Frontend.Repositories
{
    public class Repository : IRepository
    {
        private readonly HttpClient _httpClient;

        // la implementacion del repositorio  media ctor que inyecta los servicio y datos del backend mediante HttpClient conexion establecida en el program, importante implementar la prop de json para traer los datos y pasar la primera mayuscula como lo necesita csharp
        public Repository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Metodo Mapear el jseon para tener compatibilidad entre jseon y csharp por ser en minisculas a mayusculas
        private JsonSerializerOptions _jsonDefaultOptions => new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        // Metodo que consulta los datos mediante httpClient a la url del controlador api backend, serializa y despues deserializa para tener compatibilidad en los datos, con este puedo conseguir por lista de datos o por parametro
        public async Task<HttpResponseWrapper<T>> GetAsync<T>(string url)
        {
            var responseHttp = await _httpClient.GetAsync(url);
            if (responseHttp.IsSuccessStatusCode)
            {
                var response = await UnserializeAnswer<T>(responseHttp);
                return new HttpResponseWrapper<T>(response, false, responseHttp);
            }

            return new HttpResponseWrapper<T>(default, true, responseHttp);
        }

        // Metodo para crear o poner un dato mendiante el modelo es importante que pasa por el proceso de serializacion para enviarlo por el httpclient
        public async Task<HttpResponseWrapper<object>> PostAsync<T>(string url, T model)
        {
            var messageJSON = JsonSerializer.Serialize(model);
            var messageContet = new StringContent(messageJSON, Encoding.UTF8, "application/json");
            var responseHttp = await _httpClient.PostAsync(url, messageContet);
            return new HttpResponseWrapper<object>(null, !responseHttp.IsSuccessStatusCode, responseHttp);
        }

        public async Task<HttpResponseWrapper<TActionResponse>> PostAsync<T, TActionResponse>(string url, T model)
        {
            var messageJSON = JsonSerializer.Serialize(model);
            var messageContet = new StringContent(messageJSON, Encoding.UTF8, "application/json");
            var responseHttp = await _httpClient.PostAsync(url, messageContet);
            if (responseHttp.IsSuccessStatusCode)
            {
                var response = await UnserializeAnswer<TActionResponse>(responseHttp);
                return new HttpResponseWrapper<TActionResponse>(response, false, responseHttp);
            }
            return new HttpResponseWrapper<TActionResponse>(default, !responseHttp.IsSuccessStatusCode, responseHttp);
        }

        private async Task<T> UnserializeAnswer<T>(HttpResponseMessage responseHttp)
        {
            var response = await responseHttp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(response, _jsonDefaultOptions)!;
        }
    }
}