﻿using System;
using System.Collections.Generic;

namespace RecipeCubeWebService.Models;

public partial class Ingredient
{
    public int IngredientId { get; set; }

    public string? IngredientName { get; set; }

    public string? Category { get; set; }

    public string? Synonym { get; set; }

    public int? ExpireDay { get; set; }

    public string? Unit { get; set; }

    public decimal? Gram { get; set; }

    public string? Photo { get; set; }
}
