using Fantasy.Backend.Data;
using Fantasy.Backend.Helpers;
using Fantasy.Backend.Repositories.Interfaces;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entities;
using Fantasy.Shared.Responses;
using Microsoft.EntityFrameworkCore;

namespace Fantasy.Backend.Repositories.Implementations;

//1. la clase (TeamsRepository) Hereda de (GenericRepository) con una especificidad de(Team) y la interfaz de (ITeamsRepository) cabe señalar que el (GenericRepository) se ecuentra datacontex para acceder a la base de datos.
//2. Creamos el ctor de (TeamsRepository)
//3. Implementamos la interfaz de (ITeamsRepository)
public class TeamsRepository : GenericRepository<Team>, ITeamsRepository
{
    // 4.creamos el (contex) de datacontex como campo en el ctor y renombramos con (_context) para usarlo en toda la clase
    private readonly DataContext _context;

    //5.2 creamos como campo  fileStorage y renombramos con (_fileStorage) para usarlo en toda la clase
    private readonly IFileStorage _fileStorage;

    //5. Inyectamos el IFileStorage en el constructor para gestionar las imagenes de los paises y equipos
    public TeamsRepository(DataContext context, IFileStorage fileStorage) : base(context)
    {
        _context = context;
        _fileStorage = fileStorage;
    }

    //3.2 Generamos los metodos override o sobre escribirble
    // Tomar una lista de equipos
    public override async Task<ActionResponse<IEnumerable<Team>>> GetAsync()
    {
        var teams = await _context.Teams
            .Include(x => x.Country)
            .OrderBy(x => x.Name)
            .ToListAsync();
        return new ActionResponse<IEnumerable<Team>>
        {
            WasSuccess = true,
            Result = teams
        };
    }

    // tomar un equipo por parametro
    public override async Task<ActionResponse<Team>> GetAsync(int id)
    {
        var team = await _context.Teams
             .Include(x => x.Country)
             .FirstOrDefaultAsync(c => c.Id == id);

        if (team == null)
        {
            return new ActionResponse<Team>
            {
                WasSuccess = false,
                Message = "ERR001"
            };
        }

        return new ActionResponse<Team>
        {
            WasSuccess = true,
            Result = team
        };
    }

    // 6.Implementacion de paginacion

    public override async Task<ActionResponse<IEnumerable<Team>>> GetAsync(PaginationDTO pagination)
    {
        // Se obtiene una consulta sobre la tabla de equipos (Teams)
        // y se incluye la información del país asociado (Country).
        var queryable = _context.Teams
            .Include(x => x.Country)
            .AsQueryable();

        // Si el filtro de paginación no está vacío, se aplica un filtro
        // que busca los equipos cuyo país contenga el texto ingresado.
        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Country!.Name.ToLower().Contains(pagination.Filter.ToLower()));
        }

        // Se devuelve la respuesta encapsulada en un ActionResponse
        return new ActionResponse<IEnumerable<Team>>
        {
            WasSuccess = true, // Indica que la operación fue exitosa
            Result = await queryable
                .OrderBy(x => x.Name) // Ordena los equipos por nombre
                .Paginate(pagination) // Aplica la paginación según los parámetros recibidos
                .ToListAsync() // Ejecuta la consulta de manera asíncrona y obtiene la lista de equipos
        };
    }

    //3.1 Metodos propios sin herencia

    // 6.Implementacion de paginacion(No entra en el paso3)
    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination)
    {
        // Se crea una consulta sobre la tabla Teams sin ejecutarla aún
        var queryable = _context.Teams.AsQueryable();

        // Si el filtro no está vacío, se aplica un filtro de búsqueda
        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            // Filtra los equipos cuyos países contengan el texto ingresado en el filtro
            // Se usa ToLower() para hacer la búsqueda sin distinción entre mayúsculas y minúsculas
            queryable = queryable.Where(x => x.Country!.Name.ToLower().Contains(pagination.Filter.ToLower()));
        }

        // Cuenta de manera asíncrona cuántos registros cumplen con el filtro aplicado
        double count = await queryable.CountAsync();

        // Retorna el total de registros en un objeto de respuesta estructurada
        return new ActionResponse<int>
        {
            WasSuccess = true,  // Indica que la operación fue exitosa
            Result = (int)count // Se convierte el número a tipo entero antes de retornarlo
        };
    }

    // añadir un nuevo equipo
    public async Task<ActionResponse<Team>> AddAsync(TeamDTO teamDTO)
    {
        // Se valida que el CountryId exista antes de intentar crear el equipo.
        var country = await _context.Countries.FindAsync(teamDTO.CountryId);
        if (country == null)
        {
            return new ActionResponse<Team>
            {
                WasSuccess = false,
                Message = "ERR004"
            };
        }

        // creamos el objeto
        var team = new Team
        {
            Country = country,
            Name = teamDTO.Name,
        };

        // el equipo tiene imagen
        if (!string.IsNullOrEmpty(teamDTO.Image))
        {
            // convertir la imagen base64
            var imageBase64 = Convert.FromBase64String(teamDTO.Image!);
            team.Image = await _fileStorage.SaveFileAsync(imageBase64, ".jpg", "teams");
        }

        // guardamos el objeto team
        _context.Add(team);
        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<Team>
            {
                WasSuccess = true,
                Result = team
            };
        }
        catch (DbUpdateException)
        {
            return new ActionResponse<Team>
            {
                WasSuccess = false,
                Message = "ERR003"
            };
        }
        catch (Exception exception)
        {
            return new ActionResponse<Team>
            {
                WasSuccess = false,
                Message = exception.Message
            };
        }
    }

    // // Combo de equipos con la clave foránea que relaciona un equipo con un país.
    public async Task<IEnumerable<Team>> GetComboAsync(int countryId)
    {
        return await _context.Teams
            .Where(x => x.CountryId == countryId)
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    // actualizar un equipo
    public async Task<ActionResponse<Team>> UpdateAsync(TeamDTO teamDTO)
    {
        // buscar un equipo por parametro
        var currentTeam = await _context.Teams.FindAsync(teamDTO.Id);

        // si no existe saca un error
        if (currentTeam == null)
        {
            return new ActionResponse<Team>
            {
                WasSuccess = false,
                Message = "ERR005"
            };
        }

        var country = await _context.Countries.FindAsync(teamDTO.CountryId);
        if (country == null)
        {
            return new ActionResponse<Team>
            {
                WasSuccess = false,
                Message = "ERR004"
            };
        }

        if (!string.IsNullOrEmpty(teamDTO.Image))
        {
            var imageBase64 = Convert.FromBase64String(teamDTO.Image!);
            currentTeam.Image = await _fileStorage.SaveFileAsync(imageBase64, ".jpg", "teams");
        }

        currentTeam.Country = country;
        currentTeam.Name = teamDTO.Name;

        _context.Update(currentTeam);
        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<Team>
            {
                WasSuccess = true,
                Result = currentTeam
            };
        }
        catch (DbUpdateException)
        {
            return new ActionResponse<Team>
            {
                WasSuccess = false,
                Message = "ERR003"
            };
        }
        catch (Exception exception)
        {
            return new ActionResponse<Team>
            {
                WasSuccess = false,
                Message = exception.Message
            };
        }
    }
}