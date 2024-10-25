namespace RecipeCubeWebService.DTO
{
    public class RecommendRecipeDTO
    {
        public int RecipeId { get; set; }
        public string RecipeName { get; set; }
        public double MatchRate { get; set; }
        public double CoverRate { get; set; }
        public double TotalScore { get; set; }
        public string photoName { get; set; }
        public bool? Restriction { get; set; }
        public bool? WestEast { get; set; }
        public string Category { get; set; }
        public string DetailedCategory { get; set; }
        public List<int?>? IngredientIds { get; set; }
        public List<string> IngredientNames { get; set; }
        public bool IsEnoughIngredients { get; set; }  // 是否有足夠食材
        public List<MissingIngredientDTO>? MissingIngredients { get; set; } // 缺少的食材及其數量與單位
    }
}
