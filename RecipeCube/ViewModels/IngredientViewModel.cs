using System.ComponentModel.DataAnnotations;

namespace RecipeCube.ViewModels
{
    public class IngredientViewModel
    {
        public int IngredientId { get; set; }

        [Display(Name = "食材名稱")]
        public string? IngredientName { get; set; }

        [Display(Name = "食材類別")]
        public string? Category { get; set; }

        [Display(Name = "食材同義詞")]
        public string? Synonym { get; set; }

        [Display(Name = "預設到期日")]
        public int? ExpireDay { get; set; }

        [Display(Name = "單位")]
        public string? Unit { get; set; }

        [Display(Name = "每單位克數")]
        public decimal? Gram { get; set; }

        public string? Photo { get; set; }
    }
}
