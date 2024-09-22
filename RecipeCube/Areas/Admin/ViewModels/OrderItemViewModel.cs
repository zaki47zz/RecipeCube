using System.ComponentModel.DataAnnotations;

namespace RecipeCube.Areas.Admin.ViewModels;

public partial class OrderItemViewModel
{
    public int OrderItemId { get; set; }

    [Display(Name ="訂單編號")]
    public long? OrderId { get; set; }

    [Display(Name ="商品編號")]
    public int? ProductId { get; set; }

    [Display(Name ="數量")]
    public int? Quantity { get; set; }

    [Display(Name ="單價")]
    public int? Price { get; set; }
}

