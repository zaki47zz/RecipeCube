using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            var usergroups =  await _context.UserGroups.ToListAsync();

            var viewModel = usergroups.Select(usergroup => new GroupVIewModel
            {
                GroupId = usergroup.GroupId,
                GroupName = usergroup.GroupName,
                GroupAdmin = usergroup.GroupAdmin,
                GroupInvite = usergroup.GroupInvite
            });
            return PartialView("_GroupIndexPartial", viewModel);
        }

        // GET: Admin/UserGroups/Details/5
        public async Task<IActionResult> Details(int? id)
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
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userGroup = await _context.UserGroups.FindAsync(id);
            if (userGroup == null)
            {
                return NotFound();
            }
            return View(userGroup);
        }

        // POST: Admin/UserGroups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GroupId,GroupName,GroupAdmin,GroupInvite")] UserGroup userGroup)
        {
            if (id != userGroup.GroupId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userGroup);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserGroupExists(userGroup.GroupId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(userGroup);
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
