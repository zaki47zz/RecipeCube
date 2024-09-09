using System;
using System.Collections.Generic;

namespace RecipeCube.Models;

public partial class RecipeIngredient
{
    public int RecipeIngredientId { get; set; }

    public int? RecipeId { get; set; }

    public int? IngredientId { get; set; }

    public decimal? Quantity { get; set; }
}
