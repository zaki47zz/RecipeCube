using System.ComponentModel.DataAnnotations;

namespace RecipeCube.Areas.Admin.ViewModels
{
    public class ExclusiveFoodViewModel
    {
        public int ExclusiveIngredientId { get; set; }

        [Display(Name ="使用者ID")]
        public string? UserId { get; set; }

        [Display(Name ="食材ID")]
        public int? IngredientId { get; set; }
    }
}
