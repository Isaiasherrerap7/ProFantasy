using Fantasy.Backend.Data;
using Fantasy.Backend.Helpers;
using Fantasy.Backend.Repositories.Interfaces;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entities;
using Fantasy.Shared.Responses;
using Microsoft.EntityFrameworkCore;

namespace Fantasy.Backend.Repositories.Implementations;

// 1.la clase (CountriesRepository) hereda los verbos del (genericrepository) pero con una especificidad de la entidad (country) a su vez hereda los verbos del contrato de la (ICountriesRepository)
public class CountriesRepository : GenericRepository<Country>, ICountriesRepository
{
    //4..encapsulamos en datacontex con el contex como campo para usarlo en toda la solucion de la clase
    private readonly DataContext _context;

    // 2.el constructor del (CountriesRepository) al heredar el (GenericRepository) trae el DataContext lo que nos permite acceder a los datos del backend
    public CountriesRepository(DataContext context) : base(context)
    {
        _context = context;
    }

    // 5. Sobre escribimos los metodos espeficimos mediante el override para no utilizar los heredados por el generico

    // Trae los paieses por parametro  y con los equipos relacionados
    public override async Task<ActionResponse<Country>> GetAsync(int id)
    {
        var country = await _context.Countries
             .Include(c => c.Teams)
             .FirstOrDefaultAsync(c => c.Id == id);

        if (country == null)
        {
            return new ActionResponse<Country>
            {
                WasSuccess = false,
                Message = "ERR001"
            };
        }

        return new ActionResponse<Country>
        {
            WasSuccess = true,
            Result = country
        };
    }

    // Deme la colleciones de paieses e incluyame los equipos en lista
    public override async Task<ActionResponse<IEnumerable<Country>>> GetAsync()
    {
        var countries = await _context.Countries
            .Include(x => x.Teams)
            .OrderBy(x => x.Name)
            .ToListAsync();
        return new ActionResponse<IEnumerable<Country>>
        {
            WasSuccess = true,
            Result = countries
        };
    }

    // 3. implemetantos el contrato de la interfaz (ICountriesRepository)

    public async Task<IEnumerable<Country>> GetComboAsync()
    {
        return await _context.Countries
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public override async Task<ActionResponse<IEnumerable<Country>>> GetAsync(PaginationDTO pagination)
    {
        // Construye una consulta básica para obtener países,
        // incluyendo la colección de Teams asociada.
        var queryable = _context.Countries
            .Include(x => x.Teams)
            .AsQueryable();

        // Si se especifica un filtro, aplica un "Where"
        // para buscar países cuyo nombre contenga el texto indicado.
        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
        }

        // Retorna una respuesta de tipo ActionResponse<IEnumerable<Country>>,
        // marcando la operación como exitosa (WasSuccess = true).
        // Además, utiliza el método de extensión Paginate para aplicar ordenación (OrderBy por nombre)
        // y paginación (Skip y Take en base a la página y el tamaño de página).
        return new ActionResponse<IEnumerable<Country>>
        {
            WasSuccess = true,
            Result = await queryable
                .OrderBy(x => x.Name)
                .Paginate(pagination) // Salta los registros anteriores y toma los solicitados
                .ToListAsync()        // Convierte el resultado en lista asíncronamente
        };
    }

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination)
    {
        // Construye una consulta para contar cuántos países hay,
        // aplicando el mismo filtro si existe.
        var queryable = _context.Countries.AsQueryable();

        // Si se especifica un filtro, aplica el "Where" correspondiente.
        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
        }

        // Cuenta de manera asíncrona el número de registros que cumple la condición
        double count = await queryable.CountAsync();

        // Regresa la cantidad en una ActionResponse<int>.
        return new ActionResponse<int>
        {
            WasSuccess = true,
            Result = (int)count
        };
    }
}