using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeCube.Areas.Admin.ViewModels;
using RecipeCube.Models;

namespace RecipeCube.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserGroupsController : Controller
    {
        private readonly RecipeCubeContext _context;

        public UserGroupsController(RecipeCubeContext context)
        {
            _context = context;
        }

        // GET: Admin/UserGroups
        public async Task<IActionResult> GroupIndexPartial()
        {
            var usergroups = await _context.UserGroups.ToListAsync();

            var viewModel = usergroups.Select(usergroup => new GroupViewModel
            {
                GroupId = usergroup.GroupId,
                GroupName = usergroup.GroupName,
                GroupAdmin = usergroup.GroupAdmin,
                GroupInvite = usergroup.GroupInvite
            }).ToList();
            return PartialView("_GroupIndexPartial", viewModel);
        }

        // GET: Admin/UserGroups/Details/5
        public async Task<IActionResult> DetailsPartial(int id)
        {
            var usergroup = await _context.UserGroups.FirstOrDefaultAsync(m => m.GroupId == id);
            if (usergroup == null)
            {
                return NotFound();
            }

            var viewmodel = new GroupViewModel
            {
                GroupId = usergroup.GroupId,
                GroupName = usergroup.GroupName,
                GroupAdmin = usergroup.GroupAdmin,
                GroupInvite = usergroup.GroupInvite
            };
            return PartialView("_DetailsPartial", viewmodel);
        }

        // GET: Admin/UserGroups/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/UserGroups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GroupId,GroupName,GroupAdmin,GroupInvite")] UserGroup userGroup)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userGroup);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userGroup);
        }

        // GET: Admin/UserGroups/Edit/5
        [HttpGet]
        public async Task<IActionResult> EditPartial(int? id)
        {
            var usergroup = await _context.UserGroups.FindAsync(id);
            if (usergroup == null)
            {
                return NotFound();
            }
            var viewModel = new GroupViewModel
            {
                GroupId = usergroup.GroupId,
                GroupName = usergroup.GroupName,
                GroupAdmin = usergroup.GroupAdmin,
                GroupInvite = usergroup.GroupInvite
            };
            return PartialView("_EditPartial", viewModel);
        }
        // POST: Admin/UserGroups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(int id, [Bind("GroupId,GroupName,GroupAdmin,GroupInvite")] UserGroup userGroup)
        {
            if (id != userGroup.GroupId)
            {
                return new JsonResult(new { success = false, error = "ID不符合!" });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userGroup);
                    _context.SaveChanges();
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

        // GET: Admin/UserGroups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userGroup = await _context.UserGroups
                .FirstOrDefaultAsync(m => m.GroupId == id);
            if (userGroup == null)
            {
                return NotFound();
            }

            return View(userGroup);
        }

        // POST: Admin/UserGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userGroup = await _context.UserGroups.FindAsync(id);
            if (userGroup != null)
            {
                _context.UserGroups.Remove(userGroup);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserGroupExists(int id)
        {
            return _context.UserGroups.Any(e => e.GroupId == id);
        }
    }
}
