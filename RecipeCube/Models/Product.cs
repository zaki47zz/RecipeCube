using System;
using System.Collections.Generic;

namespace RecipeCube.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string? ProductName { get; set; }

    public int? IngredientId { get; set; }

    public int? Price { get; set; }

    public int? Stock { get; set; }

    public bool? Status { get; set; }

    public string? Photo { get; set; }

    public decimal? UnitQuantity { get; set; }

    public string? Description { get; set; }
}
