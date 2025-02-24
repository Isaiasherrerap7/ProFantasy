using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entities;
using Fantasy.Shared.Responses;

namespace Fantasy.Backend.Repositories.Interfaces;

public interface ICountriesRepository
{
    // contrato para el coutriones
    // traer pais por paramtrero
    Task<ActionResponse<Country>> GetAsync(int id);

    // traer paieses
    Task<ActionResponse<IEnumerable<Country>>> GetAsync();

    // Combo de equipo
    Task<IEnumerable<Country>> GetComboAsync();

    //  Lista de paginacion
    Task<ActionResponse<IEnumerable<Country>>> GetAsync(PaginationDTO pagination);

    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);
}