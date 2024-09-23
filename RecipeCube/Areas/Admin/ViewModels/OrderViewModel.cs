using System.ComponentModel.DataAnnotations;

namespace RecipeCube.Areas.Admin.ViewModels;

    public partial class OrderViewModel
    {
        [Display(Name ="訂單編號")]
        [Required(ErrorMessage = "訂單編號欄位必填")]
        public long OrderId { get; set; }

        [Display(Name="使用者編號")]
        [Required(ErrorMessage = "使用編號欄位必填")]
        public string? UserId { get; set; }

        [Display(Name="訂單時間")]
        [Required(ErrorMessage ="訂單時間必填")]
        public DateTime? OrderTime { get; set; }
        [Display(Name = "訂單總價")]
        [Required(ErrorMessage ="訂單總價欄位必填")]
        public int? TotalAmount { get; set; }
        
        [Display(Name = "訂單狀態")]
        [Required(ErrorMessage ="訂單狀態欄位必填")]
        public bool? Status { get; set; }
    }
