using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeCubeWebService.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using RecipeCubeWebService.DTO;

namespace RecipeCubeWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailSender _emailSender;  // 注入 IEmailSender 服務
        public EmailController(IEmailSender emailSender)
        {
            _emailSender = emailSender;  // 注入郵件寄送服務
        }

        // POST: api/Email/Send
        [HttpPost("Send")]
        public async Task<IActionResult> SendEmail([FromBody] EmailRequestDTO emailRequest)
        {
            // 驗證輸入資料
            if (string.IsNullOrWhiteSpace(emailRequest.Email) || string.IsNullOrWhiteSpace(emailRequest.Message))
            {
                return BadRequest(new { Message = "收件人和信件內容不可為空。" });
            }

            // 寄送郵件
            await _emailSender.SendEmailAsync(emailRequest.Email, "通知", emailRequest.Message);

            // 返回成功回應
            return Ok(new { Message = "信件已成功寄出。" });
        }
    }
}
