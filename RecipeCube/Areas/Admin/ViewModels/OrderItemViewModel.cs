using System.ComponentModel.DataAnnotations;

namespace RecipeCube.Areas.Admin.ViewModels;

public partial class OrderItemViewModel
{
    [Display(Name ="訂單細項編號")]
    [Required(ErrorMessage ="訂單細項編號欄位必填")]
    public int OrderItemId { get; set; }

    [Display(Name ="訂單編號")]
    [Required(ErrorMessage ="訂單編號欄位必填")]
    public long? OrderId { get; set; }

    [Display(Name ="商品編號")]
    [Required(ErrorMessage ="商品編號欄位必填")]
    public int? ProductId { get; set; }

    [Display(Name ="商品名稱")]
    public string? ProductName { get; set; } // 商品名稱

    [Display(Name ="數量")]
    [Required(ErrorMessage ="數量欄位必填")]
    public int? Quantity { get; set; }

    [Display(Name ="單價")]
    [Required(ErrorMessage ="單價欄位必填")]
    public int? Price { get; set; }

    
}

