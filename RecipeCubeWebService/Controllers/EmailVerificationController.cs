using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RecipeCubeWebService.DTO;
using RecipeCubeWebService.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RecipeCubeWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailVerificationController : ControllerBase
    {
        private readonly RecipeCubeContext _context; // 注入資料庫上下文
        private readonly string _jwtSecret = "thisisaverylongsecretkeyforjwtwhichis256bits!!"; // 替換為您的 JWT 秘鑰
        public EmailVerificationController(RecipeCubeContext context) // 構造函數
        {
            _context = context;
        }

        // POST: api/EmailVerification/GenerateToken
        // 生成 Token 方法
        [HttpPost("GenerateToken")]
        public IActionResult GenerateEmailVerificationToken([FromBody] UserVerificationDTO user)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.Email))
            {
                return BadRequest("Invalid user information.");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] {
            new Claim("email", user.Email) // 使用字符串 "email"
        }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            var verificationLink = Url.Action("VerifyEmail", "EmailVerification", new { token = tokenString }, Request.Scheme);
            Console.WriteLine($"生成的 Token: {tokenString}");
            return Ok(new { VerificationLink = verificationLink });
        }

        // 這個 action 用來接受驗證請求
        [HttpGet("VerifyEmail")]
        public IActionResult VerifyEmail(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest("Invalid token.");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.FromMinutes(5)
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var email = jwtToken.Claims.First(x => x.Type == "email").Value; // 更新為使用字符串 "email"

                // 查找用戶並更新 EmailConfirmed 狀態
                var user = _context.Users.SingleOrDefault(u => u.Email == email);
                if (user == null)
                {
                    return NotFound(new { Message = "User not found." });
                }

                user.EmailConfirmed = true; // 更新 EmailConfirmed 狀態
                _context.Users.Update(user); // 更新用戶
                _context.SaveChanges(); // 保存更改

                return Ok(new { Message = "Email verified successfully", Email = email });
            }
            catch (SecurityTokenExpiredException)
            {
                return Unauthorized(new { Message = "Token expired" });
            }
            catch (SecurityTokenInvalidSignatureException)
            {
                return Unauthorized(new { Message = "Invalid token signature" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error validating token: {ex.GetType().Name} - {ex.Message}");
                return Unauthorized(new { Message = "Invalid token" });
            }
        }
    }
}
