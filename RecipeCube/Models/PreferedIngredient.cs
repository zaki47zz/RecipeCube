using System;
using System.Collections.Generic;

namespace RecipeCube.Models;

public partial class PreferedIngredient
{
    public int PerferIngredientId { get; set; }

    public string? UserId { get; set; }

    public int? IngredientId { get; set; }
}
