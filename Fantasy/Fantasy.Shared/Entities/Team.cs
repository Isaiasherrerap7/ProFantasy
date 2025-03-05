using Fantasy.Shared.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy.Shared.Entities;

public class Team
{
    public int Id { get; set; }

    [Display(Name = "Team", ResourceType = typeof(Literals))]
    [MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string Name { get; set; } = null!;

    public string? Image { get; set; }

    // Un equipo pertenece a un pais
    public Country? Country { get; set; }

    // Es la clave foránea que relaciona un equipo con un país.
    [Display(Name = "Country", ResourceType = typeof(Literals))]
    public int CountryId { get; set; }

    [Display(Name = "IsImageSquare", ResourceType = typeof(Literals))]
    public bool IsImageSquare { get; set; }

    // prop de lectura si no ecnuentra una image poner la poder defecto
    public string ImageFull => string.IsNullOrEmpty(Image) ? "/images/NoImage.png" : Image;
}