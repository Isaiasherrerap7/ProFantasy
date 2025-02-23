using Fantasy.Shared.DTOs;

namespace Fantasy.Backend.Helpers
{
    // Clase estática que extiende las funcionalidades de IQueryable<T>
    // para aplicar paginación de manera sencilla.
    public static class QueryableExtensions
    {
        // Método de extensión "Paginate" que, a partir de un IQueryable<T> inicial,
        // salta un número determinado de registros (Skip) y toma un número fijo de registros (Take),
        // basándose en los valores de la clase PaginationDTO.
        // Esto permite obtener los resultados de una página específica con un tamaño definido.
        public static IQueryable<T> Paginate<T>(this IQueryable<T> queryable, PaginationDTO pagination)
        {
            // 'Skip' descarta los registros anteriores a la página actual,
            // calculado mediante: (página actual - 1) * registros por página.
            // 'Take' luego selecciona la cantidad de registros especificados.
            return queryable
                .Skip((pagination.Page - 1) * pagination.RecordsNumber)
                .Take(pagination.RecordsNumber);
        }
    }
}