using System;
using System.Collections.Generic;

namespace RecipeCube.Models;

public partial class User
{
    public int UserId { get; set; }

    public string? Username { get; set; }

    public int? GroupId { get; set; }

    public string? Password { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public bool? DietaryRestrictions { get; set; }

    public bool? PreferredChecked { get; set; }

    public bool? ExclusiveChecked { get; set; }

    public bool? Status { get; set; }
}
