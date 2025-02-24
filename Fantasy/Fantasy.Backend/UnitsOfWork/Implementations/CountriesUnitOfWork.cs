using Fantasy.Backend.Repositories.Interfaces;
using Fantasy.Backend.UnitsOfWork.Interfaces;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entities;
using Fantasy.Shared.Responses;

namespace Fantasy.Backend.UnitsOfWork.Implementations;

// la clas(CountriesUnitOfWork) hereda los verbos dle (GenericUnitOfWork) con una especificidad de Country e implementa la interfaz de (ICountriesUnitOfWork)
public class CountriesUnitOfWork : GenericUnitOfWork<Country>, ICountriesUnitOfWork
{
    // 4. agregamos como campo (_countriesRepository) para ususarlo en la solucion de la clase
    private readonly ICountriesRepository _countriesRepository;

    // 1. Generamos el constructor (CountriesUnitOfWork)
    // 3.Inyectamos el repositorio especifico de country (ICountriesRepository) para hacer uno de los verbos con una capa en la unidad de trabajo
    public CountriesUnitOfWork(IGenericRepository<Country> repository, ICountriesRepository countriesRepository) : base(repository)
    {
        _countriesRepository = countriesRepository;
    }

    // 5. sobre escribimos los metodos especificos que no queremos usar los genericos de la UW
    // 6. la unidad de trabajo es un orquestador entre el repositorio especifico de country y el controller que lo va utilizar para no llegar a los datos de la base de datos directamente

    public override async Task<ActionResponse<IEnumerable<Country>>> GetAsync() => await _countriesRepository.GetAsync();

    public override async Task<ActionResponse<Country>> GetAsync(int id) => await _countriesRepository.GetAsync(id);

    public override async Task<ActionResponse<IEnumerable<Country>>> GetAsync(PaginationDTO pagination) => await _countriesRepository.GetAsync(pagination);

    // 2. implementamos la interfaz(ICountriesUnitOfWork)
    public async Task<IEnumerable<Country>> GetComboAsync() => await _countriesRepository.GetComboAsync();

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _countriesRepository.GetTotalRecordsAsync(pagination);
}