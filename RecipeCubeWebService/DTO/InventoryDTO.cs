using System.ComponentModel.DataAnnotations;

namespace RecipeCubeWebService.DTO
{
    public class InventoryDTO
    {
        public int InventoryId { get; set; }

        public int GroupId { get; set; }

        public string UserId { get; set; }

        public int? IngredientId { get; set; }

        public string IngredientName { get; set; }

        public string Category { get; set; }

        public string Synonym { get; set; }

        public string Unit { get; set; }

        public decimal? Gram { get; set; }

        public string Photo { get; set; }

        public decimal? Quantity { get; set; }

        private DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);
        private DateOnly _expiryDate = DateOnly.MinValue;
        public DateOnly? ExpiryDate
        {
            get => _expiryDate; // "=>"是{return xxx} 的簡寫式
            set
            {
                _expiryDate = value ?? _expiryDate; //沒有值則維持最小值
                UpdateIsExpiring(); //透過下面的method運算一下有沒有要過期
            }
        }

        public bool? IsExpiring { get; set; } = true;

        public bool? Visibility { get; set; }

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
    }
}
