using System.ComponentModel.DataAnnotations;

namespace RecipeCube.Areas.Admin.ViewModels
{
    public class IngredientViewModel
    {
        public int IngredientId { get; set; }

        [Display(Name = "食材名稱")]
        [Required(ErrorMessage = "食材名稱為必填欄位")]
        [StringLength(255, ErrorMessage = "名稱不能超過255個字元")]
        public string? IngredientName { get; set; }

        [Display(Name = "食材類別")]
        [Required(ErrorMessage = "食材類別為必填欄位")]
        [StringLength(100, ErrorMessage = "名稱不能超過100個字元")]
        public string? Category { get; set; }

        [Display(Name = "食材同義詞")]
        [StringLength(255, ErrorMessage = "名稱不能超過255個字元")]
        public string? Synonym { get; set; }

        [Display(Name = "預設到期日")]
        public int? ExpireDay { get; set; } = 7; //預設到期日

        [Display(Name = "單位")]
        public string? Unit { get; set; }

        [Display(Name = "每單位克數")]
        [Range(0.1, 10000.00, ErrorMessage = "單位克數必須在0.01到10000之間")]
        [DisplayFormat(DataFormatString = "{0:F1}", ApplyFormatInEditMode = true)] // 格式化為一位小數
        public decimal? Gram { get; set; }

        [Display(Name = "照片")]
        public string? Photo { get; set; }
    }
}
