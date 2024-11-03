
namespace RecipeCubeWebService.DTO
{
    public class RandomRecommendReciepDTO
    {
        public int RecipeId { get; set; }
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

        public string? Description { get; set; }

        // 食材名稱
        public List<string> SelectedIngredientNames { get; set; } = new List<string>();

        // 食材數量
        public Dictionary<int, decimal> IngredientQuantities { get; set; } = new Dictionary<int, decimal>();

        // 食材的單位
        public Dictionary<int, string> IngredientUnits { get; set; } = new Dictionary<int, string>();

        // 缺少的食材
        public List<MissingIngredientDTO>? MissingIngredients { get; set; } = new List<MissingIngredientDTO>();

    }

}
