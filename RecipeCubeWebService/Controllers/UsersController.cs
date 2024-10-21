using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class UsersController : ControllerBase
    {
        private readonly RecipeCubeContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;


        public UsersController(RecipeCubeContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        // GET: api/Users/5
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

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }
        // POST: api/Users
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
        public IActionResult SignUpDTO(SignUpDTO signUp)
        {
            var existingUser = _context.Users.SingleOrDefault(u => u.Email == signUp.Email);
            if (existingUser != null)
            {
                return Conflict(new { Message = "User with this email already exists" });
            }


            var userName = signUp.Email;
            var newUser = new User
            {
                Id = Guid.NewGuid().ToString(), // 隨機生成UUID
                UserName = userName, // 使用者名稱設為 email
                NormalizedUserName = userName.ToUpper(), // 正常化名稱
                Email = signUp.Email, // 註冊的Email
                NormalizedEmail = signUp.Email.ToUpper(), // 正常化Email
                EmailConfirmed = true, // 設為已確認電子郵件
                SecurityStamp = Guid.NewGuid().ToString(), // 隨機生成
                ConcurrencyStamp = Guid.NewGuid().ToString(), // 隨機生成
                PhoneNumber = "", // 註冊時的手機號碼（可選）
                PhoneNumberConfirmed = false, // 手機號碼未確認
                TwoFactorEnabled = false, // 預設關閉雙重驗證
                LockoutEnabled = true, // 開啟鎖定功能
                AccessFailedCount = 0, // 登入失敗次數設為0
                DietaryRestrictions = signUp.DietaryRestrictions, // 其他自訂字段
                ExclusiveChecked = false,
                GroupId = 0,
                PreferredChecked = false,
                Status = true // 使用者狀態設定為啟用
            };


            newUser.PasswordHash = _passwordHasher.HashPassword(newUser, signUp.Password);


            try
            {
                // 4. 將新使用者保存至資料庫
                _context.Users.Add(newUser);
                _context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (UserExists(newUser.Id))
                {
                    return Conflict(new { Message = "User with this ID already exists" });
                }
                else
                {
                    throw; // 如果是其他的資料庫錯誤，則拋出異常
                }
            }


            // 5. 返回註冊成功的回應
            return Ok(new { Message = "User created successfully", User = newUser });


        }


        // 登入功能
        [HttpPost("SignIn")]
        public IActionResult SignIn(SignInDTO signIn)
        {
            var user = _context.Users.SingleOrDefault(u => u.Email == signIn.Email);
            if (user != null)
            {
                var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, signIn.Password);
                if (passwordVerificationResult == PasswordVerificationResult.Success)
                {
                    // 創建 JWT Token
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes("3f50a33d7bca2e1a346f5a8c4f5b7e9a");
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                   {
                            new Claim(ClaimTypes.SerialNumber, user.Id),
                            new Claim(ClaimTypes.Name, user.UserName),
                       new Claim(ClaimTypes.Email, user.Email)
                   }),
                        // 測試用設定token到期日為30天
                        Expires = DateTime.UtcNow.AddDays(30),
                        //Expires = DateTime.UtcNow.AddSeconds(30),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
                    };


                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    var tokenString = tokenHandler.WriteToken(token);
                    return Ok(new { Token = tokenString, Message = "登入成功", usernamejwt = user.UserName });
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
