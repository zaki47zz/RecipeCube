using System.ComponentModel.DataAnnotations;
namespace RecipeCubeWebService.DTO
{
    public class InventoryDTO
    {
        public int InventoryId { get; set; }
        public int GroupId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int? IngredientId { get; set; }
        public string IngredientName { get; set; }
        public string Category { get; set; }
        public string Synonym { get; set; }
        public string Unit { get; set; }
        public decimal? Gram { get; set; }
        public string Photo { get; set; }
        public decimal? Quantity { get; set; }
        public DateOnly? ExpiryDate { get; set; }
        public bool? Visibility { get; set; }
    }
}