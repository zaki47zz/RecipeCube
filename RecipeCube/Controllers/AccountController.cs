using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RecipeCube.Data;
using System.Threading.Tasks;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string token, string email)
    {
        // 根據 email 找到使用者
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return View("Error"); // 顯示錯誤頁面
        }

        // 驗證 Email
        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (result.Succeeded)
        {
            return View("ConfirmEmail"); // 驗證成功後顯示的頁面
        }

        return View("Error"); // 驗證失敗顯示錯誤頁面
    }
}
