using System;
using System.Collections.Generic;

namespace RecipeCubeWebService.Models;

public partial class PreferedIngredient
{
    public int PreferIngredientId { get; set; }

    public string? UserId { get; set; }

    public int? IngredientId { get; set; }
}
