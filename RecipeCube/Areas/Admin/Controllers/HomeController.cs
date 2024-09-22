using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RecipeCube.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        // 要求Roles ="Admin"的使用者登入才能訪問Admin頁面
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
