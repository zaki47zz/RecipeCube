using System.ComponentModel.DataAnnotations.Schema;
namespace RecipeCubeWebService.Models
{
    public partial class PreferedIngredient
    {
        public virtual Ingredient Ingredient { get; set; } // 這樣在重生後也不會消失
        // 拼寫錯誤 懶得重生 先這樣應急
        [Column("PerferIngredientId")]
        public int PreferIngredientId { get; set; }
    }
}