using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace RecipeCube.Areas.Admin.ViewModels
{
    public class RecipeIngredientViewModel
    {

        public int RecipeIngredientId { get; set; }
        [Display(Name = "食譜Id")]
        public int? RecipeId { get; set; }
        [Display(Name = "食材Id")]
        public int? IngredientId { get; set; }
        [Display(Name = "食材數量")]
        public decimal? Quantity { get; set; }
        [Display(Name = "食譜名稱")]
        public string RecipeName { get; set; } // 食譜名稱
        [Display(Name = "食材名稱")]
        public string IngredientName { get; set; } // 食材名稱

        public string Unit { get; set; } // 食材單位
    }
}
