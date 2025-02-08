using System.ComponentModel.DataAnnotations;

namespace Fantasy.Shared.Entities;

public class Country
{
    public int Id { get; set; }

    [MaxLength(100)]
    [Required]
    public string Name { get; set; } = null!;

    // un pais tiene varios equipos
    public ICollection<Team>? Teams { get; set; }

    // Propiedad de lectura solo se puede optener
    public int TeamsCount => Teams == null ? 0 : Teams.Count;
}