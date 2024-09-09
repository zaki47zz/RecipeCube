using System;
using System.Collections.Generic;

namespace RecipeCube.Models;

public partial class Recipe
{
    public int RecipeId { get; set; }

    public string? RecipeName { get; set; }

    public int? UserId { get; set; }

    public bool? IsCustom { get; set; }

    public bool? Restriction { get; set; }

    public bool? WestEast { get; set; }

    public string? Category { get; set; }

    public string? DetailedCategory { get; set; }

    public string? Steps { get; set; }

    public string? Seasoning { get; set; }

    public bool? Visibility { get; set; }

    public string? Photo { get; set; }

    public bool? Status { get; set; }
}
