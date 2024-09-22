using Microsoft.AspNetCore.Mvc.ModelBinding;
using RecipeCube.Models;
using System.ComponentModel.DataAnnotations;

namespace RecipeCube.Areas.Admin.ViewModels
{
    public class InventoryViewModel
    {
        public int InventoryId { get; set; }

        [Display(Name = "群組ID")]
        [Required(ErrorMessage = "群組為必填欄位")]
        public int? GroupId { get; set; }

        [Display(Name = "群組名稱")]
        public string? GroupName { get; set; }

        [Display(Name = "使用者ID")]
        [Required(ErrorMessage = "使用者為必填欄位")]
        public string? UserId { get; set; }

        [Display(Name = "使用者名稱")]
        public string? UserName { get; set; }

        [Display(Name = "食材ID")]
        public int? IngredientId { get; set; }

        [Display(Name = "食材名稱")]
        public string? IngredientName { get; set; }

        [Display(Name = "食材單位")]
        public string? IngredientUnit { get; set; }

        [Display(Name = "數量")]
        [Range(0.1, 10000.00, ErrorMessage = "數量需介在0.1到10000之間")]
        [DisplayFormat(DataFormatString = "{0:F1}", ApplyFormatInEditMode = true)] // 格式化為一位小數
        public decimal? Quantity { get; set; }

        private DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);
        private DateOnly _expiryDate = DateOnly.MinValue;

        [Display(Name = "到期日")]
        public DateOnly? ExpiryDate 
        {
            get => _expiryDate; // "=>"是{return xxx} 的簡寫式
            set
            {
                _expiryDate = value ?? _expiryDate; //沒有值則維持最小值
                UpdateIsExpiring(); //透過下面的method運算一下有沒有要過期
            }
        }

        [Display(Name = "即將過期")]
        public bool? IsExpiring { get; set; }

        [Display(Name = "即將過期")]
        public string? IsExpiringStr
        {
            get
            {
                if (IsExpiring == true)
                {
                    return "是";
                }
                else if (IsExpiring == false)
                {
                    return "否";
                }
                else //null的話
                {
                    return "未知";
                }
            }
        }

        [Display(Name = "可見性")]
        public bool? Visibility { get; set; } = true;

        [Display(Name = "可見性")]
        public string? VisibilityStr
        {
            get
            {
                if (Visibility == true)
                {
                    return "群組可見";
                }
                else if (Visibility == false)
                {
                    return "僅本人可見";
                }
                else //null的話
                {
                    return "無設定";
                }
            }
        }
        private void UpdateIsExpiring()
        {
            if (_expiryDate == DateOnly.MinValue)
            {
                IsExpiring = null; //如果_expiryDate維持在預設值
            }
            else
            {
                //日期差值
                int daysUntilExpiry = _expiryDate.DayNumber - currentDate.DayNumber;
                IsExpiring = daysUntilExpiry <= 3;
            }
        }

        public List<UserGroup>? Groups { get; set; }
        public List<User>? Users { get; set; }

        // 新增一個屬性來儲存所有食材
        [BindNever]
        [Display(Name = "可供選擇的食材")]
        public List<IngredientViewModel>? AvailableIngredients { get; set; }

        // 使用者選擇的食材清單
        [Display(Name = "選擇食材")]
        public List<int> SelectedIngredients { get; set; } = new List<int>();

        // 食材數量
        [Display(Name = "食材數量")]
        public Dictionary<int, decimal> IngredientQuantities { get; set; } = new Dictionary<int, decimal>();

        // 食材到期日
        [Display(Name = "食材到期日")]
        public Dictionary<int, DateOnly> IngredientExpiryDate { get; set; } = new Dictionary<int, DateOnly>();

        // 食材即將到期
        [Display(Name = "食材即將到期")]
        public Dictionary<int, bool> IngredientIsExpiring { get; set; } = new Dictionary<int, bool>();

        [Display(Name = "食材可見性")]
        public Dictionary<int, bool> IngredientVisibility { get; set; } = new Dictionary<int, bool>();

        // 保存食材的單位
        [BindNever]
        [Display(Name = "食材單位")]
        public Dictionary<int, string> IngredientUnits { get; set; } = new Dictionary<int, string>();
    }
}
