namespace RecipeCube.Models;

public partial class UserGroup
{
    public int GroupId { get; set; }

    public string? GroupName { get; set; }

    public int? GroupAdmin { get; set; }

    public int? GroupInvite { get; set; }
}
