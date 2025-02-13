using Fantasy.Shared.Resources;
using System.ComponentModel.DataAnnotations;

namespace Fantasy.Shared.Entities;

public class Country
{
    public int Id { get; set; }

    [Display(Name = "Country", ResourceType = typeof(Literals))]
    [MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string Name { get; set; } = null!;

    // un pais tiene una coleccion de varios equipos
    public ICollection<Team>? Teams { get; set; }

    // Propiedad de lectura solo se puede optener Es una propiedad calculada que devuelve la cantidad de equipos asociados a un país.
    public int TeamsCount => Teams == null ? 0 : Teams.Count;
}