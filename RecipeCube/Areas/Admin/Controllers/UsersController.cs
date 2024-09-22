using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeCube.Areas.Admin.ViewModels;
using RecipeCube.Models;

namespace RecipeCube.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly RecipeCubeContext _context;

        public UsersController(RecipeCubeContext context)
        {
            _context = context;
        }

        // GET: Admin/Users
        public async Task<IActionResult> UserIndexPartial()
        {
            var users = await _context.Users.ToListAsync();

            var viewModel = users.Select(user => new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumber = user.PhoneNumber,
                PreferredChecked = user.PreferredChecked,
                DietaryRestrictions = user.DietaryRestrictions,
                ExclusiveChecked = user.ExclusiveChecked,
                GroupId = user.GroupId,
                Status = user.Status
            }).ToList();

            return PartialView("_UserIndexPartial", viewModel);
        }

        // GET: Admin/Users/Details/5
        public async Task<IActionResult> DetailsPartial(string id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var viemodel =  new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumber = user.PhoneNumber,
                PreferredChecked = user.PreferredChecked,
                DietaryRestrictions = user.DietaryRestrictions,
                ExclusiveChecked = user.ExclusiveChecked,
                GroupId = user.GroupId,
                Status = user.Status
            };
            return PartialView("_DetailsPartial", viemodel);
        }

        // GET: Admin/Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount,DietaryRestrictions,ExclusiveChecked,GroupId,PreferredChecked,Status")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Admin/Users/Edit/5
        [HttpGet]
        public async Task<IActionResult> EditPartial(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var viewModel =  new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                PreferredChecked = user.PreferredChecked,
                DietaryRestrictions = user.DietaryRestrictions,
                ExclusiveChecked = user.ExclusiveChecked,
                GroupId = user.GroupId,
                Status = user.Status
            };

            return PartialView("_EditPartial", viewModel);
        }

        // POST: Admin/Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Edit(string id, [Bind("Id,UserName,Email,PhoneNumber,DietaryRestrictions,ExclusiveChecked,GroupId,PreferredChecked")] UserViewModel userViewModel)
        {
            if (id != userViewModel.Id)
            {
                return new JsonResult(new { success = false, error = "ID不符合!" });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // 取得原始資料
                    var user = await _context.Users.FindAsync(id);
                    if (user == null)
                    {
                        return new JsonResult(new { success = false, error = "找不到使用者!" });
                    }

                    // 更新指定的欄位
                    user.UserName = userViewModel.UserName;
                    user.Email = userViewModel.Email;
                    user.PhoneNumber = userViewModel.PhoneNumber;
                    user.DietaryRestrictions = userViewModel.DietaryRestrictions;
                    user.ExclusiveChecked = userViewModel.ExclusiveChecked;
                    user.GroupId = userViewModel.GroupId;
                    user.PreferredChecked = userViewModel.PreferredChecked;

                    // 標記已修改的欄位
                    _context.Entry(user).Property(u => u.UserName).IsModified = true;
                    _context.Entry(user).Property(u => u.Email).IsModified = true;
                    _context.Entry(user).Property(u => u.PhoneNumber).IsModified = true;
                    _context.Entry(user).Property(u => u.DietaryRestrictions).IsModified = true;
                    _context.Entry(user).Property(u => u.ExclusiveChecked).IsModified = true;
                    _context.Entry(user).Property(u => u.GroupId).IsModified = true;
                    _context.Entry(user).Property(u => u.PreferredChecked).IsModified = true;

                    await _context.SaveChangesAsync();
                    return new JsonResult(new { success = true });
                }
                catch (DbUpdateConcurrencyException)
                {
                    return new JsonResult(new { success = false, error = "發生錯誤!" });
                }
            }
            return new JsonResult(new
            {
                success = false,
                error = "資料無效!",
                errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
            });
        }


        // GET: Admin/Users/Delete/5
        public async Task<IActionResult> DeletePartial(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var viewmodel = new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumber = user.PhoneNumber,
                PreferredChecked = user.PreferredChecked,
                DietaryRestrictions = user.DietaryRestrictions,
                ExclusiveChecked = user.ExclusiveChecked,
                GroupId = user.GroupId,
                Status = user.Status
            };
            return PartialView("_DeletePartial", viewmodel);
        }

        // POST: Admin/Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Delete(string id, [Bind("Id,Status")] UserViewModel userViewModel)
        {
            if (id != userViewModel.Id)
            {
                return new JsonResult(new { success = false, error = "ID不符合!" });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // 取得原始資料
                    var user = await _context.Users.FindAsync(id);
                    if (user == null)
                    {
                        return new JsonResult(new { success = false, error = "找不到使用者!" });
                    }

                    // 更新指定的欄位
                    user.Status = userViewModel.Status;

                    // 標記已修改的欄位
                    _context.Entry(user).Property(u => u.Status).IsModified = true;

                    await _context.SaveChangesAsync();
                    return new JsonResult(new { success = true });
                }
                catch (DbUpdateConcurrencyException)
                {
                    return new JsonResult(new { success = false, error = "發生錯誤!" });
                }
            }
            return new JsonResult(new
            {
                success = false,
                error = "資料無效!",
                errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
            });
        }


        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
