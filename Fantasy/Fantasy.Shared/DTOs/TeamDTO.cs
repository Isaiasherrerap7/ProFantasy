﻿using Fantasy.Shared.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy.Shared.DTOs;

public class TeamDTO
{
    public int Id { get; set; }

    [Display(Name = "Team", ResourceType = typeof(Literals))]
    [MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string Name { get; set; } = null!;

    [Display(Name = "Image", ResourceType = typeof(Literals))]
    public string? Image { get; set; }

    [Display(Name = "Country", ResourceType = typeof(Literals))]
    public int CountryId { get; set; }

    [Display(Name = "IsImageSquare", ResourceType = typeof(Literals))]
    public bool IsImageSquare { get; set; }
}