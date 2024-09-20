using System;
using System.Collections.Generic;

namespace RecipeCube.Models;

public partial class ExclusiveIngredient
{
    public int ExclusiveIngredientId { get; set; }

    public string? UserId { get; set; }

    public int? IngredientId { get; set; }
}
