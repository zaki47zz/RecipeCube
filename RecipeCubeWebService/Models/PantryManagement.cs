using System;
using System.Collections.Generic;

namespace RecipeCubeWebService.Models;

public partial class PantryManagement
{
    public int PantryId { get; set; }

    public int? GroupId { get; set; }

    public string? OwnerId { get; set; }

    public string? UserId { get; set; }

    public int? IngredientId { get; set; }

    public decimal? Quantity { get; set; }

    public string? Action { get; set; }

    public DateTime? Time { get; set; }
}
