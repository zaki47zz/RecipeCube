using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace RecipeCubeWebService.DTO
{
    public class RecipeDto
    {
        public int RecipeId { get; set; }

        [Required(ErrorMessage = "食譜名稱為必填")]
        [MaxLength(10, ErrorMessage = "食譜名稱長度不可超過10個字")]
        public string RecipeName { get; set; }

        public string UserId { get; set; }

        public bool? IsCustom { get; set; }

        public bool? Restriction { get; set; }

        public bool? WestEast { get; set; }

        public string Category { get; set; }

        public string DetailedCategory { get; set; }

        public string Steps { get; set; }

        public string Seasoning { get; set; }

        public int? Visibility { get; set; }

        public string? PhotoName { get; set; }

        public bool Status { get; set; }
        public string Time { get; set; }
        public string UserName { get; set; }

        public string? Description { get; set; }
        // 使用者選擇的食材清單
        [Required(ErrorMessage = "請選擇至少一種食材")]
        public List<int> SelectedIngredients { get; set; } = new List<int>();
        // 食材名稱
        public List<string> SelectedIngredientNames { get; set; } = new List<string>();
        // 新增同義字欄位
        public List<string> Synonyms { get; set; } = new List<string>(); 
        // 食材數量
        [Required(ErrorMessage = "請填寫食材數量")]
        public Dictionary<int, decimal> IngredientQuantities { get; set; } = new Dictionary<int, decimal>();

        // 食材的單位
        public Dictionary<int, string> IngredientUnits { get; set; } = new Dictionary<int, string>();
    }

}
