using Fantasy.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fantasy.Backend.Data;

// validar si existe base de datos si no la crea y carga los paieses de Countries
public class SeedDb
{
    private readonly DataContext _context;

    public SeedDb(DataContext context)
    {
        _context = context;
    }

    // Si la base de dato no existe realiza la migracion
    public async Task SeedAsync()
    {
        await _context.Database.EnsureCreatedAsync();
        await CheckCountriesAsync();
        await CheckTeamsAsync();
    }

    // validar si hay paises si no los carga del archivo sql
    private async Task CheckCountriesAsync()
    {
        if (!_context.Countries.Any())
        {
            var countriesSQLScript = File.ReadAllText("Data\\Countries.sql");
            await _context.Database.ExecuteSqlRawAsync(countriesSQLScript);
        }
    }

    private async Task CheckTeamsAsync()
    {
        if (!_context.Teams.Any())
        {
            foreach (var country in _context.Countries)
            {
                _context.Teams.Add(new Team { Name = country.Name, Country = country! });
                if (country.Name == "Colombia")
                {
                    _context.Teams.Add(new Team { Name = "Medellín", Country = country! });
                    _context.Teams.Add(new Team { Name = "Nacional", Country = country! });
                    _context.Teams.Add(new Team { Name = "Millonarios", Country = country! });
                    _context.Teams.Add(new Team { Name = "Junior", Country = country! });
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}