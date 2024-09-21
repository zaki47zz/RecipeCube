using System.ComponentModel.DataAnnotations;

namespace RecipeCube.Areas.Admin.ViewModels
{
    public class GroupViewModel
    {
        
        public int GroupId { get; set; }

        [Display(Name = "群組名稱")]
        public string? GroupName { get; set; }

        [Display(Name = "群組創建者ID")]
        public string? GroupAdmin { get; set; }

        [Display(Name = "群組邀請碼")]
        public int? GroupInvite { get; set; }
    }
}
