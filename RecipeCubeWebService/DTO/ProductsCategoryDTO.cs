namespace RecipeCubeWebService.DTO
{
    public class ProductsCategoryDTO
    {
        public int ProductId { get; set; }

        public int? IngredientId { get; set; }

        public string? Category { get; set; }

        public int Count {  get; set; }
    }
}
