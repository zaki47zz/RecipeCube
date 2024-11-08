﻿using System;
using System.Collections.Generic;

namespace RecipeCubeWebService.Models;

public partial class Coupon
{
    public int CouponId { get; set; }

    public string? CouponName { get; set; }

    public int? Status { get; set; }

    public decimal? DiscountValue { get; set; }

    public string? DiscountType { get; set; }

    public int? MinSpend { get; set; }
}
