namespace RecipeCubeWebService.DTO
{
    public class RecommendRecipeRequestDTO
    {
        public string UserId { get; set; }
        public List<int> SelectedIngredients { get; set; }
    }
}