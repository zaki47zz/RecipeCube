using System;
using System.Collections.Generic;

namespace RecipeCubeWebService.Models;

public partial class Order
{
    public long OrderId { get; set; }

    public string? UserId { get; set; }

    public DateTime? OrderTime { get; set; }

    public int? TotalAmount { get; set; }

    public int? Status { get; set; }

    public string? OrderAddress { get; set; }

    public string? OrderPhone { get; set; }

    public string? OrderEmail { get; set; }

    public string? OrderRemark { get; set; }

    public string? OrderName { get; set; }
}
