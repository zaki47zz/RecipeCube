using RecipeCube.Models;
using System.ComponentModel.DataAnnotations;

namespace RecipeCube.Areas.Admin.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }
        [Display(Name = "使用者名稱")]
        public string? UserName { get; set; }
        public string? NormalizedUserName { get; set; }
        public string? Email { get; set; }
        public string? NormalizedEmail { get; set; }

        [Display(Name = "Email驗證")]
        public bool EmailConfirmed { get; set; }

        public string? PasswordHash { get; set; }
        public string? SecurityStamp { get; set; }
        public string? ConcurrencyStamp { get; set; }

        [Display(Name = "電話")]
        public string? PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }


        [Display(Name = "飲食限制")]
        public bool DietaryRestrictions { get; set; }

        [Display(Name = "偏好篩選")]
        public bool ExclusiveChecked { get; set; }

        [Display(Name = "不可食用食材篩選")]
        public bool PreferredChecked { get; set; }
        [Display(Name = "群組Id")]
        public int GroupId { get; set; }
        [Display(Name = "會員狀態")]
        public bool Status { get; set; }
        public virtual ICollection<UserLogin> UserLogins { get; set; } = new List<UserLogin>();
    }
}
