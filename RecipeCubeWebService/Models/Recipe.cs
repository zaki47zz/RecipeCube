﻿using System;
using System.Collections.Generic;

namespace RecipeCubeWebService.Models;

public partial class Recipe
{
    public int RecipeId { get; set; }

    public string? RecipeName { get; set; }

    public string? UserId { get; set; }

    public bool? IsCustom { get; set; }

    public bool? Restriction { get; set; }

    public bool? WestEast { get; set; }

    public string? Category { get; set; }

    public string? DetailedCategory { get; set; }

    public string? Steps { get; set; }

    public string? Seasoning { get; set; }

    public int? Visibility { get; set; }

    public string? Photo { get; set; }

    public bool? Status { get; set; }

    public string? Time { get; set; }

    public string? Description { get; set; }
}
