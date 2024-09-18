namespace RecipeCube.Models;

public partial class PreferedIngredient
{
    public int PerferIngredientId { get; set; }

    public int? UserId { get; set; }

    public int? IngredientId { get; set; }
}
