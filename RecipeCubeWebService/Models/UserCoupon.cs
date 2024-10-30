using System;
using System.Collections.Generic;

namespace RecipeCubeWebService.Models;

public partial class UserCoupon
{
    public int UserConponId { get; set; }

    public string? UserId { get; set; }

    public int? CouponId { get; set; }

    public int? Status { get; set; }

    public DateOnly? AcquireDate { get; set; }
}
