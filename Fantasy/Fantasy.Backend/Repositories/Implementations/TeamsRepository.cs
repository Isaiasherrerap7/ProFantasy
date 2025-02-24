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

    public override async Task<ActionResponse<IEnumerable<Team>>> GetAsync(PaginationDTO pagination)
    {
        var queryable = _context.Teams
            .Include(x => x.Country)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Country!.Name.ToLower().Contains(pagination.Filter.ToLower()));
        }

        return new ActionResponse<IEnumerable<Team>>
        {
            WasSuccess = true,
            Result = await queryable
                .OrderBy(x => x.Name)
                .Paginate(pagination)
                .ToListAsync()
        };
    }

    //3.1 Metodos propios sin herencia

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination)
    {
        var queryable = _context.Teams.AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Country!.Name.ToLower().Contains(pagination.Filter.ToLower()));
        }

        double count = await queryable.CountAsync();
        return new ActionResponse<int>
        {
            WasSuccess = true,
            Result = (int)count
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