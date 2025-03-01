using Fantasy.Backend.Repositories.Interfaces;
using Fantasy.Backend.UnitsOfWork.Interfaces;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entities;
using Fantasy.Shared.Responses;

namespace Fantasy.Backend.UnitsOfWork.Implementations;

// 1. la clase (TeamsUnitOfWork) hereda la clase generica (GenericUnitOfWork) y su especificidad (Team) y hereda la interfaz (ITeamsUnitOfWork)
//2. Generamos el constructor de (TeamsUnitOfWork) por gener el (GenericUnitOfWork) el ctor inyecta el repository donde esta la logica de la entidad
// 3. Implementa la interfaz del (ITeamsUnitOfWork)
// 4. Inyecta el repositorio especifico en el ctor (ITeamsRepository)
public class TeamsUnitOfWork : GenericUnitOfWork<Team>, ITeamsUnitOfWork
{
    // 5. Generamos teamsRepository como campo
    //6. fijarnos cuantos metodos implemento la interfaz para sobre escribir los heredados
    private readonly ITeamsRepository _teamsRepository;

    public TeamsUnitOfWork(IGenericRepository<Team> repository, ITeamsRepository teamsRepository) : base(repository)
    {
        _teamsRepository = teamsRepository;
    }

    // 7.establecemos que los metodos no sean los genericos si no los de la logica del repository espeficico (teamsRepository)
    // 8.inyectar el repository y la unitofwork en el program
    //9. ya puedo crear la api con el contexto de (TeamsUnitOfWork)

    public override async Task<ActionResponse<IEnumerable<Team>>> GetAsync() => await _teamsRepository.GetAsync();

    public override async Task<ActionResponse<Team>> GetAsync(int id) => await _teamsRepository.GetAsync(id);

    public override async Task<ActionResponse<IEnumerable<Team>>> GetAsync(PaginationDTO pagination) => await _teamsRepository.GetAsync(pagination);

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _teamsRepository.GetTotalRecordsAsync(pagination);

    public async Task<ActionResponse<Team>> AddAsync(TeamDTO teamDTO) => await _teamsRepository.AddAsync(teamDTO);

    public async Task<IEnumerable<Team>> GetComboAsync(int countryId) => await _teamsRepository.GetComboAsync(countryId);

    public async Task<ActionResponse<Team>> UpdateAsync(TeamDTO teamDTO) => await _teamsRepository.UpdateAsync(teamDTO);
}