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
        private DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);
        private DateOnly? _expiryDate;
        public DateOnly? ExpiryDate
        {
            get => _expiryDate;
            set
            {
                _expiryDate = value;
                UpdateIsExpiring();
            }
        }
        public bool? IsExpiring { get; private set; } //確保只能通過 UpdateIsExpiring 方法來更新
        public bool? IsExpired { get; private set; }
        public bool? Visibility { get; set; }
        private void UpdateIsExpiring()
        {
            if (_expiryDate == null)
            {
                IsExpiring = null;
            }
            else
            {
                int daysUntilExpiry = _expiryDate.Value.DayNumber - currentDate.DayNumber;
                IsExpiring = daysUntilExpiry <= 3 && daysUntilExpiry >= 0;
                IsExpired = daysUntilExpiry < 0;
            }
        }
    }
}