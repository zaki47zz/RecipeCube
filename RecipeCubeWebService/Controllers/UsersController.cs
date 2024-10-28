using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RecipeCubeWebService.DTO;
using RecipeCubeWebService.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Net.Http;
using System.Net.Http.Json;


namespace RecipeCubeWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly RecipeCubeContext _context;
        // 偷內建hash方法，注入後就能拿來用了
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly HttpClient _httpClient;

        public UsersController(RecipeCubeContext context, IPasswordHasher<User> passwordHasher, HttpClient httpClient)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _httpClient = httpClient;
        }

        // GET: api/Users 測試GET有沒有壞掉用
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        /*  購物車會用到  */
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return user;
        }
        /*  購物車會用到  */

        // POST: api/Users 測試POST有沒有壞掉用
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            _context.Users.Add(user);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UserExists(user.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }


            return Ok(new { Message = "User created successfully", User = user });
        }


        // 註冊功能
        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUpDTO(SignUpDTO signUp)
        {
            if (signUp == null)
            {
                return BadRequest("Invalid signup data.");
            }
            var existingUser = await _context.Users.SingleOrDefaultAsync(u => u.Email == signUp.Email);
            if (existingUser != null)
            {
                return Conflict(new { Message = "User with this email already exists" });
            }

            var userName = signUp.Email;
            var newUser = new User
            {
                Id = Guid.NewGuid().ToString(), // 隨機生成UUID
                UserName = userName, // 使用者名稱預設為 email
                NormalizedUserName = userName.ToUpper(), // 正常化名稱
                Email = signUp.Email, // 註冊的Email
                NormalizedEmail = signUp.Email.ToUpper(), // 正常化Email
                EmailConfirmed = false, // 設為未確認電子郵件
                SecurityStamp = Guid.NewGuid().ToString(), // 隨機生成
                ConcurrencyStamp = Guid.NewGuid().ToString(), // 隨機生成
                PhoneNumber = "", // 註冊時的手機號碼
                PhoneNumberConfirmed = false, // 手機號碼未確認
                TwoFactorEnabled = false, // 預設關閉雙重驗證
                LockoutEnabled = true, // 開啟鎖定功能
                AccessFailedCount = 0, // 登入失敗次數設為0
                DietaryRestrictions = signUp.DietaryRestrictions, //葷(F)素(T)選擇，前端預設F
                ExclusiveChecked = false, // 預設不可食用食物關閉，驗證後可在使用者設定修改填入
                GroupId = 0, // 預設沒有群組
                PreferredChecked = false, // 預設偏好食物關閉，驗證後可在使用者設定修改填入
                Status = true // 使用者狀態設定為啟用
            };

            // 用偷來的方法hash加密password
            newUser.PasswordHash = _passwordHasher.HashPassword(newUser, signUp.Password);

            // 將新使用者保存至資料庫
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            var emailVerificationEndpoint = "https://localhost:7188/api/EmailVerification/GenerateToken";
            var emailVerificationRequest = new
            {
                Email = signUp.Email
            };

            var response = await _httpClient.PostAsJsonAsync(emailVerificationEndpoint, emailVerificationRequest);
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, new { Message = "Failed to generate verification link." });
            }

            var verificationData = await response.Content.ReadFromJsonAsync<VerificationResponseDTO>();

            // 呼叫 Email API 發送郵件
            var emailSendEndpoint = "https://localhost:7188/api/Email/Send";
            var emailSendRequest = new
            {
                Email = signUp.Email,
                Message = $"請點擊以下連結以驗證您的帳號：{verificationData?.VerificationLink}"
            };

            var emailResponse = await _httpClient.PostAsJsonAsync(emailSendEndpoint, emailSendRequest);
            if (!emailResponse.IsSuccessStatusCode)
            {
                return StatusCode((int)emailResponse.StatusCode, new { Message = "Failed to send verification email." });
            }

            return Ok(new
            {
                Email = signUp.Email,
                VerificationLink = verificationData?.VerificationLink,
                Message = "Verification email has been sent."
            });
        }

        // 登入功能  


        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn(SignInDTO signIn)
        {
            var user = _context.Users.SingleOrDefault(u => u.Email == signIn.Email);

            if (user != null)
            {
                var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, signIn.Password);

                if (passwordVerificationResult == PasswordVerificationResult.Success)
                {
                    // 創建 JWT Token
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes("thisisaverylongsecretkeyforjwtwhichis256bits!!");

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                            new Claim(ClaimTypes.SerialNumber, user.Id),
                            new Claim(ClaimTypes.Email, user.Email),
                            new Claim(ClaimTypes.Name, user.UserName),
                            new Claim(ClaimTypes.GroupSid, user.GroupId.ToString()),
                        }),
                        Expires = DateTime.UtcNow.AddDays(30),
                        SigningCredentials = new SigningCredentials(
                            new SymmetricSecurityKey(key),
                            SecurityAlgorithms.HmacSha256Signature)
                    };

                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    var tokenString = tokenHandler.WriteToken(token);

                    return Ok(new
                    {
                        Token = tokenString,
                        Message = "登入成功",
                        usernamejwt = user.UserName
                    });
                }
            }

            return Ok(new { Message = "登入失敗" });
        }


        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }

}
