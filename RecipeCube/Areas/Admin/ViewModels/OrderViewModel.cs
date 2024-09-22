using System.ComponentModel.DataAnnotations;

namespace RecipeCube.Areas.Admin.ViewModels;

    public partial class OrderViewModel
    {
        [Display(Name ="訂單編號")]
        public long OrderId { get; set; }

        [Display(Name="使用者編號")]
        public string? UserId { get; set; }

        [Display(Name="訂單時間")]
        public DateTime? OrderTime { get; set; }

        [Display(Name = "訂單總價")]
        public int? TotalAmount { get; set; }

        [Display(Name = "訂單狀態")]
        public bool? Status { get; set; }
    }
