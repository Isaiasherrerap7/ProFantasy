using Fantasy.Backend.Data;
using Fantasy.Backend.UnitsOfWork.Interfaces;
using Fantasy.Shared.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fantasy.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CountriesController : GenericController<Country>
{
    public CountriesController(IGenericUnitOfWork<Country> unit) : base(unit)
    {
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