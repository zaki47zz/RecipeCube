using System.ComponentModel.DataAnnotations;

namespace RecipeCube.Areas.Admin.ViewModels
{
    public class InventoryViewModel
    {
        public int InventoryId { get; set; }

        [Display(Name = "群組ID")]
        public int? GroupId { get; set; }

        [Display(Name = "使用者ID")]
        public int? UserId { get; set; }

        [Display(Name = "食材ID")]
        public int? IngredientId { get; set; }

        [Display(Name = "數量")]
        public decimal? Quantity { get; set; }

        [Display(Name = "到期日")]
        public DateOnly? ExpiryDate { get; set; }

        [Display(Name = "即將過期")]
        public bool? IsExpiring { get; set; }

        [Display(Name = "可見性")]
        public bool? Visibility { get; set; }
    }
}
