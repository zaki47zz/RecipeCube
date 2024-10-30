namespace RecipeCubeWebService.DTO
{
    public class CouponsDTO
    {
        public int CouponId { get; set; }

        public string CouponName { get; set; }

        public int? CouponStatus { get; set; }

        public decimal? DiscountValue { get; set; }

        public string DiscountType { get; set; }

        public int UserConponId { get; set; }

        public string UserId { get; set; }


        public int? UsedStatus { get; set; }

        public DateOnly? AcquireDate { get; set; }

        public int? MinSpend { get; set; }
    }
}
