using System.ComponentModel.DataAnnotations;

namespace RecipeCube.Areas.Admin.ViewModels;

public partial class ProductViewModel
{
    [Display(Name ="商品編號")]
    public int ProductId { get; set; }

    [Display(Name="商品名稱")]
    public string? ProductName { get; set; }

    [Display(Name ="食材編號")]
    public int? IngredientId { get; set; }

    [Display(Name ="商品價格")]
    public int? Price { get; set; }

    [Display(Name ="庫存")]
    public int? Stock { get; set; }

    [Display(Name ="商品狀態")]
    public bool? Status { get; set; }

    [Display(Name ="商品圖檔名稱")]
    public string? Photo { get; set; }
}
