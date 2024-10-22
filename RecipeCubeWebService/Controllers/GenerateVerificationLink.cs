using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using RecipeCubeWebService.Models; // 根據你的 User 模型 namespace 調整
using System.Threading.Tasks;

namespace RecipeCubeWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenerateVerificationLink : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public GenerateVerificationLink(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> CreateVerificationLink([FromBody] string email)
        {
            // 查找使用者
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound(new { message = "使用者不存在。" });
            }

            // 生成驗證 token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token }, Request.Scheme);

            return Ok(new { verificationLink = link });
        }
    }
}
