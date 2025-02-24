using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entities;
using Fantasy.Shared.Responses;

namespace Fantasy.Backend.UnitsOfWork.Interfaces;

public interface ICountriesUnitOfWork
{
    // contrato para el coutriones
    // traer pais por paramtrero
    Task<ActionResponse<Country>> GetAsync(int id);

    // traer paies por paramtrero
    Task<ActionResponse<IEnumerable<Country>>> GetAsync();

    // Combo de equipo
    Task<IEnumerable<Country>> GetComboAsync();

    //  Lista de paginacion
    Task<ActionResponse<IEnumerable<Country>>> GetAsync(PaginationDTO pagination);

    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);
}