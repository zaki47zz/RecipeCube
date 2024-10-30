using System;
using System.Collections.Generic;

namespace RecipeCubeWebService.Models;

public partial class UserGroup
{
    public int GroupId { get; set; }

    public string? GroupName { get; set; }

    public string? GroupAdmin { get; set; }

    public int? GroupInvite { get; set; }
}
