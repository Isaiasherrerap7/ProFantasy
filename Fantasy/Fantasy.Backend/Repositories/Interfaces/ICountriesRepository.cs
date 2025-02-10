using Fantasy.Shared.Entities;
using Fantasy.Shared.Responses;

namespace Fantasy.Backend.Repositories.Interfaces;

public interface ICountriesRepository
{
    // contrato para el coutriones
    // traer pais por paramtrero
    Task<ActionResponse<Country>> GetAsync(int id);

    // traer paies por paramtrero
    Task<ActionResponse<IEnumerable<Country>>> GetAsync();

    // Combo de equipo
    Task<IEnumerable<Country>> GetComboAsync();
}