using System;
using System.Collections.Generic;

namespace RecipeCubeWebService.Models;

public partial class UserIdMapping
{
    public int? OldUserId { get; set; }

    public string NewUserId { get; set; } = null!;
}
