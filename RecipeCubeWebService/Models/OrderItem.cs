﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace RecipeCubeWebService.Models;

public partial class OrderItem
{
    public int OrderItemId { get; set; }

    public long? OrderId { get; set; }

    public int? ProductId { get; set; }

    public int? Quantity { get; set; }

    public int? Price { get; set; }
}