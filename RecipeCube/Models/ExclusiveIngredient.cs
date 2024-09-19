namespace RecipeCube.Models;

public partial class ExclusiveIngredient
{
    public int ExclusiveIngredientId { get; set; }

    public int? UserId { get; set; }

    public int? IngredientId { get; set; }
}
