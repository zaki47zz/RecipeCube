using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RecipeCube.Data;

namespace RecipeCube.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        // 利用依賴注入將 UserManager 注入
        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> SeedUsers()
        {
            var users = new List<ApplicationUser>
            {
              new ApplicationUser
    {
        UserName = "admin@example.com",
        Email = "admin@example.com",
        PhoneNumber = "0912345678",
        group_id = 1,
        dietary_restrictions = false,
        preferred_checked = false,
        exclusive_checked = false,
        status = true,
        EmailConfirmed = true
    },
    new ApplicationUser
    {
        UserName = "user2@example.com",
        Email = "user2@example.com",
        PhoneNumber = "0911123456",
        group_id = 3,
        dietary_restrictions = false,
        preferred_checked = false,
        exclusive_checked = false,
        status = true,
        EmailConfirmed = true
    },
    new ApplicationUser
    {
        UserName = "user3@example.com",
        Email = "user3@example.com",
        PhoneNumber = "0922234567",
        group_id = 2,
        dietary_restrictions = false,
        preferred_checked = false,
        exclusive_checked = false,
        status = true,
        EmailConfirmed = true
    },
    new ApplicationUser
    {
        UserName = "user4@example.com",
        Email = "user4@example.com",
        PhoneNumber = "0933345678",
        group_id = 4,
        dietary_restrictions = false,
        preferred_checked = false,
        exclusive_checked = false,
        status = true,
        EmailConfirmed = true
    },
    new ApplicationUser
    {
        UserName = "user5@example.com",
        Email = "user5@example.com",
        PhoneNumber = "0944456789",
        group_id = 5,
        dietary_restrictions = false,
        preferred_checked = false,
        exclusive_checked = false,
        status = true,
        EmailConfirmed = true
    },
    new ApplicationUser
    {
        UserName = "user6@example.com",
        Email = "user6@example.com",
        PhoneNumber = "0955567890",
        group_id = 2,
        dietary_restrictions = false,
        preferred_checked = false,
        exclusive_checked = false,
        status = true,
        EmailConfirmed = true
    },
    new ApplicationUser
    {
        UserName = "user7@example.com",
        Email = "user7@example.com",
        PhoneNumber = "0966678901",
        group_id = 2,
        dietary_restrictions = false,
        preferred_checked = false,
        exclusive_checked = false,
        status = true,
        EmailConfirmed = true
    },
    new ApplicationUser
    {
        UserName = "user8@example.com",
        Email = "user8@example.com",
        PhoneNumber = "0977789012",
        group_id = 3,
        dietary_restrictions = false,
        preferred_checked = false,
        exclusive_checked = false,
        status = true,
        EmailConfirmed = true
    },
    new ApplicationUser
    {
        UserName = "user9@example.com",
        Email = "user9@example.com",
        PhoneNumber = "0988890123",
        group_id = 4,
        dietary_restrictions = false,
        preferred_checked = false,
        exclusive_checked = false,
        status = true,
        EmailConfirmed = true
    },
    new ApplicationUser
    {
        UserName = "user10@example.com",
        Email = "user10@example.com",
        PhoneNumber = "0999901234",
        group_id = 5,
        dietary_restrictions = false,
        preferred_checked = false,
        exclusive_checked = false,
        status = true,
        EmailConfirmed = true
    },
    new ApplicationUser
    {
        UserName = "user11@example.com",
        Email = "user11@example.com",
        PhoneNumber = "0912123456",
        group_id = 3,
        dietary_restrictions = false,
        preferred_checked = false,
        exclusive_checked = false,
        status = true,
        EmailConfirmed = true
    },
    new ApplicationUser
    {
        UserName = "user12@example.com",
        Email = "user12@example.com",
        PhoneNumber = "0922234567",
        group_id = 2,
        dietary_restrictions = false,
        preferred_checked = false,
        exclusive_checked = false,
        status = true,
        EmailConfirmed = true
    },
    new ApplicationUser
    {
        UserName = "user13@example.com",
        Email = "user13@example.com",
        PhoneNumber = "0933345678",
        group_id = 3,
        dietary_restrictions = false,
        preferred_checked = false,
        exclusive_checked = false,
        status = true,
        EmailConfirmed = true
    },
    new ApplicationUser
    {
        UserName = "user14@example.com",
        Email = "user14@example.com",
        PhoneNumber = "0944456789",
        group_id = 4,
        dietary_restrictions = false,
        preferred_checked = false,
        exclusive_checked = false,
        status = true,
        EmailConfirmed = true
    },
    new ApplicationUser
    {
        UserName = "user15@example.com",
        Email = "user15@example.com",
        PhoneNumber = "0955567890",
        group_id = 5,
        dietary_restrictions = false,
        preferred_checked = false,
        exclusive_checked = false,
        status = true,
        EmailConfirmed = true
    },
    new ApplicationUser
    {
        UserName = "user16@example.com",
        Email = "user16@example.com",
        PhoneNumber = "0966678901",
        group_id = 4,
        dietary_restrictions = false,
        preferred_checked = false,
        exclusive_checked = false,
        status = true,
        EmailConfirmed = true
    },
    new ApplicationUser
    {
        UserName = "user17@example.com",
        Email = "user17@example.com",
        PhoneNumber = "0977789012",
        group_id = 2,
        dietary_restrictions = false,
        preferred_checked = false,
        exclusive_checked = false,
        status = true,
        EmailConfirmed = true
    },
    new ApplicationUser
    {
        UserName = "user18@example.com",
        Email = "user18@example.com",
        PhoneNumber = "0988890123",
        group_id = 3,
        dietary_restrictions = false,
        preferred_checked = false,
        exclusive_checked = false,
        status = true,
        EmailConfirmed = true
    },
    new ApplicationUser
    {
        UserName = "user19@example.com",
        Email = "user19@example.com",
        PhoneNumber = "0999901234",
        group_id = 4,
        dietary_restrictions = false,
        preferred_checked = false,
        exclusive_checked = false,
        status = true,
        EmailConfirmed = true
    },
    new ApplicationUser
    {
        UserName = "user20@example.com",
        Email = "user20@example.com",
        PhoneNumber = "0912123456",
        group_id = 5,
        dietary_restrictions = false,
        preferred_checked = false,
        exclusive_checked = false,
        status = true,
        EmailConfirmed = true
    },
    new ApplicationUser
    {
        UserName = "user21@example.com",
        Email = "user21@example.com",
        PhoneNumber = "0922234567",
        group_id = 5,
        dietary_restrictions = false,
        preferred_checked = false,
        exclusive_checked = false,
        status = true,
        EmailConfirmed = true
    },
    new ApplicationUser
    {
        UserName = "user22@example.com",
        Email = "user22@example.com",
        PhoneNumber = "0933345678",
        group_id = 2,
        dietary_restrictions = false,
        preferred_checked = false,
        exclusive_checked = false,
        status = true,
        EmailConfirmed = true
    },
    new ApplicationUser
    {
        UserName = "user23@example.com",
        Email = "user23@example.com",
        PhoneNumber = "0944456789",
        group_id = 3,
        dietary_restrictions = false,
        preferred_checked = false,
        exclusive_checked = false,
        status = true,
        EmailConfirmed = true
    },
    new ApplicationUser
    {
        UserName = "user24@example.com",
        Email = "user24@example.com",
        PhoneNumber = "0955567890",
        group_id = 4,
        dietary_restrictions = false,
        preferred_checked = false,
        exclusive_checked = false,
        status = true,
        EmailConfirmed = true
    },
    new ApplicationUser
    {
        UserName = "user25@example.com",
        Email = "user25@example.com",
        PhoneNumber = "0966678901",
        group_id = 5,
        dietary_restrictions = false,
        preferred_checked = false,
        exclusive_checked = false,
        status = true,
        EmailConfirmed = true
    },
    new ApplicationUser
    {
        UserName = "user26@example.com",
        Email = "user26@example.com",
        PhoneNumber = "0977789012",
        group_id = 2,
        dietary_restrictions = false,
        preferred_checked = false,
        exclusive_checked = false,
        status = true,
        EmailConfirmed = true
    },
    new ApplicationUser
    {
        UserName = "user27@example.com",
        Email = "user27@example.com",
        PhoneNumber = "0988890123",
        group_id = 2,
        dietary_restrictions = false,
        preferred_checked = false,
        exclusive_checked = false,
        status = true,
        EmailConfirmed = true
    },
    new ApplicationUser
    {
        UserName = "user28@example.com",
        Email = "user28@example.com",
        PhoneNumber = "0999901234",
        group_id = 3,
        dietary_restrictions = false,
        preferred_checked = false,
        exclusive_checked = false,
        status = true,
        EmailConfirmed = true
    },
    new ApplicationUser
    {
        UserName = "user29@example.com",
        Email = "user29@example.com",
        PhoneNumber = "0912123456",
        group_id = 4,
        dietary_restrictions = false,
        preferred_checked = false,
        exclusive_checked = false,
        status = true,
        EmailConfirmed = true
    },
    new ApplicationUser
    {
        UserName = "user30@example.com",
        Email = "user30@example.com",
        PhoneNumber = "0922234567",
        group_id = 5,
        dietary_restrictions = false,
        preferred_checked = false,
        exclusive_checked = false,
        status = true,
        EmailConfirmed = true
    }


            };

            foreach (var user in users)
            {
                var existingUser = await _userManager.FindByEmailAsync(user.UserName);
                if (existingUser == null)
                {
                    var result = await _userManager.CreateAsync(user, "Password123!");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, $"使用者 {user.Email} 已經存在。");
                }
            }
            return Ok("已成功建立使用者");
        }

        public async Task<IActionResult> CreateAdminRoleAndUser()
        {
            // 確保 RoleManager 被注入
            var roleManger = HttpContext.RequestServices.GetRequiredService<RoleManager<IdentityRole>>();

            // 創建 "Admin" 角色
            if (!await roleManger.RoleExistsAsync("Admin"))
            {
                await roleManger.CreateAsync(new IdentityRole("Admin"));
            }

            // 找到要設為 Admin 的使用者
            var user = await _userManager.FindByEmailAsync("admin@example.com");

            // 將使用者加入 "Admin" 角色
            if (user != null && !await _userManager.IsInRoleAsync(user, "Admin"))
            {
                await _userManager.AddToRoleAsync(user, "Admin");
            }
            return Ok("Admin腳色已成功新增");
        }
    }

}
