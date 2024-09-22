using System.ComponentModel.DataAnnotations;

namespace RecipeCube.Areas.Admin.ViewModels
{
    public class PantryViewModel
    {
        public int PantryId { get; set; }

        [Display(Name = "群組ID")]
        public int? GroupId { get; set; }

        [Display(Name = "群組名稱")]
        public string? GroupName { get; set; }

        [Display(Name = "使用者ID")]
        public string? UserId { get; set; }

        [Display(Name = "使用者名稱")]
        public string? UserName { get; set; }

        [Display(Name = "食材ID")]
        public int? IngredientId { get; set; }

        [Display(Name = "食材名稱")]
        public string? IngredientName { get; set; }

        [Display(Name = "食材單位")]
        public string? IngredientUnit { get; set; }

        [Display(Name = "數量")]
        public decimal? Quantity { get; set; }

        [Display(Name = "行動")]
        public string? Action { get; set; }

        [Display(Name = "時間")]
        public DateTime? Time { get; set; }
    }
}
