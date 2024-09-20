using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace RecipeCube.Areas.Admin.ViewModels
{
    public class RecipeViewModel
    {

        public int RecipeId { get; set; }
        [Display(Name = "食譜名稱")]
        public string? RecipeName { get; set; }
        [Display(Name = "使用者ID")]
        public string? UserId { get; set; }
        [Display(Name = "自訂食譜")]
        public bool? IsCustom { get; set; }
        [Display(Name = "葷素")]
        public bool? Restriction { get; set; }
        [Display(Name = "中西式")]
        public bool? WestEast { get; set; }
        [Display(Name = "種類")]
        public string? Category { get; set; }
        [Display(Name = "種類細項")]
        public string? DetailedCategory { get; set; }
        [Display(Name = "烹飪步驟")]
        public string? Steps { get; set; }
        [Display(Name = "調味料")]
        public string? Seasoning { get; set; }
        [Display(Name = "可見性")]
        public bool? Visibility { get; set; }
        [Display(Name = "食譜照片")]
        public string? Photo { get; set; }
        [Display(Name = "食譜狀態")]
        public bool? Status { get; set; }

        // 新增一個屬性來儲存所有食材
        // 可供選擇的食材清單
        [BindNever]
        [Display(Name = "可供選擇的食材")]
        public List<IngredientViewModel>? AvailableIngredients { get; set; }

        // 使用者選擇的食材清單
        [Display(Name = "選擇的食材")]
        public List<int> SelectedIngredients { get; set; } = new List<int>(); // 這裡保存已選擇食材的ID
        [Display(Name = "食材數量")]
        public Dictionary<int, decimal> IngredientQuantities { get; set; } = new Dictionary<int, decimal>();
    }
}
