// ExclusiveIngredientExtensions.cs
namespace RecipeCubeWebService.Models
{
    public partial class ExclusiveIngredient
    {
        public virtual Ingredient Ingredient { get; set; } // 這樣在重生後也不會消失
    }
}