using Fantasy.Backend.Data;
using Fantasy.Backend.Repositories.Interfaces;
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
}