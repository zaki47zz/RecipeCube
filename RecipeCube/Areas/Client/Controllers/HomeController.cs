using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RecipeCube.Client.Controllers
{
    [Area("Client")]
    public class HomeController : Controller
    {
        // 要求使用者登入才能訪問頁面
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
    }
}
