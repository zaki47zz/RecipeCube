using System;
using System.Collections.Generic;

namespace RecipeCube.Models;

public partial class Inventory
{
    public int InventoryId { get; set; }

    public int? GroupId { get; set; }

    public string? UserId { get; set; }

    public int? IngredientId { get; set; }

    public decimal? Quantity { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public bool? IsExpiring { get; set; }

    public bool? Visibility { get; set; }
}
