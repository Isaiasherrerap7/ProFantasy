using Fantasy.Backend.Data;
using Fantasy.Backend.UnitsOfWork.Interfaces;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fantasy.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CountriesController : GenericController<Country>
{
    // 2. generamos como campo(ICountriesUnitOfWork _countriesUnitOfWork) para utilizarlo en toda la solucion
    private readonly ICountriesUnitOfWork _countriesUnitOfWork;

    //1. Ctor donde inyectamos  la interfaz(IGenericUnitOfWork) y lo especificamos la entidad (Country)
    // y la interfaz (ICountriesUnitOfWork countriesUnitOfWork) encargada de orquestar la unidad repositorio especifico donde esta la logica y operaciones de los verbos
    public CountriesController(IGenericUnitOfWork<Country> unit, ICountriesUnitOfWork countriesUnitOfWork) : base(unit)
    {
        _countriesUnitOfWork = countriesUnitOfWork;
    }

    // otros override

    [HttpGet("paginated")]
    public override async Task<IActionResult> GetAsync(PaginationDTO pagination)
    {
        var response = await _countriesUnitOfWork.GetAsync(pagination);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }

    // 4. Sobre escribimos los meotodos que necesitamos
    [HttpGet]
    public override async Task<IActionResult> GetAsync()
    {
        var response = await _countriesUnitOfWork.GetAsync();
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }

    [HttpGet("{id}")]
    public override async Task<IActionResult> GetAsync(int id)
    {
        var response = await _countriesUnitOfWork.GetAsync(id);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return NotFound(response.Message);
    }

    [HttpGet("totalRecordsPaginated")]
    public async Task<IActionResult> GetTotalRecordsAsync([FromQuery] PaginationDTO pagination)
    {
        var action = await _countriesUnitOfWork.GetTotalRecordsAsync(pagination);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return BadRequest();
    }

    // 3. Implementamos los meotodos especificos

    [HttpGet("combo")]
    public async Task<IActionResult> GetComboAsync()
    {
        return Ok(await _countriesUnitOfWork.GetComboAsync());
    }
}

// implentacion v1
//public class CountriesController : ControllerBase

//{
//    // Constructor para utilizar datacontex y sus entidades en toda la solucion
//    private readonly DataContext _context;

//    public CountriesController(DataContext context)
//    {
//        _context = context;
//    }

//    // Lista de objetos
//    [HttpGet]
//    public async Task<IActionResult> GetAsync()
//    {
//        return Ok(await _context.Countries.ToListAsync());
//    }

//    // Buscar por parametro
//    [HttpGet("{id}")]
//    public async Task<IActionResult> GetAsync(int id)
//    {
//        var country = await _context.Countries.FirstOrDefaultAsync(c => c.Id == id);

//        // Si no esta el pais
//        if (country == null)
//        {
//            return NotFound();
//        }

//        return Ok(country);
//    }

//    // Crear objeto
//    [HttpPost]
//    public async Task<IActionResult> PostAsync(Country country)
//    {
//        _context.Add(country);
//        await _context.SaveChangesAsync();
//        return Ok(country);
//    }

//    // Actualizar
//    [HttpPut]
//    public async Task<IActionResult> PutAsync(Country country)
//    {
//        _context.Update(country);
//        await _context.SaveChangesAsync();
//        return Ok(country);
//    }

//    // BORRAR
//    [HttpDelete("{id}")]
//    public async Task<IActionResult> DeleteAsync(int id)
//    {
//        var country = await _context.Countries.FirstOrDefaultAsync(c => c.Id == id);
//        if (country == null)
//        {
//            return NotFound();
//        }

//        _context.Remove(country);
//        await _context.SaveChangesAsync();
//        return NoContent();
//    }
//}