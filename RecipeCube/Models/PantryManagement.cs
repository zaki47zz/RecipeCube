using System;
using System.Collections.Generic;

namespace RecipeCube.Models;

public partial class PantryManagement
{
    public int PantryId { get; set; }

    public int? GroupId { get; set; }

    public int? UserId { get; set; }

    public int? IngredientId { get; set; }

    public decimal? Quantity { get; set; }

    public bool? OutOfStock { get; set; }

    public string? Action { get; set; }

    public DateTime? Time { get; set; }
}
