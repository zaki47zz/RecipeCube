using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RecipeCube.Models;

namespace RecipeCube.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RecipesController : Controller
    {
        private readonly RecipeCubeContext _context;

        public RecipesController(RecipeCubeContext context)
        {
            _context = context;
        }

        // GET: Admin/Recipes
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.Recipes.ToListAsync());
        //}

        public async Task<IActionResult> RecipeIndexPartial()
        {
            var recipes = await _context.Recipes.ToListAsync();
            return PartialView("_RecipeIndexPartial", recipes);
        }


        // GET: Admin/Recipes/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var recipe = await _context.Recipes
        //        .FirstOrDefaultAsync(m => m.RecipeId == id);
        //    if (recipe == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(recipe);
        //}
        // 新增的部分：RecipesController.cs
        public async Task<IActionResult> DetailsPartial(int id)
        {
            var recipe = await _context.Recipes
                .FirstOrDefaultAsync(m => m.RecipeId == id);

            if (recipe == null)
            {
                return NotFound();
            }

            return PartialView("_DetailsPartial", recipe);
        }
        // GET: Admin/Recipes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Recipes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RecipeId,RecipeName,UserId,IsCustom,Restriction,WestEast,Category,DetailedCategory,Steps,Seasoning,Visibility,Photo,Status")] Recipe recipe)
        {
            if (ModelState.IsValid)
            {
                _context.Add(recipe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(recipe);
        }

        // GET: Admin/Recipes/Edit/5
        public async Task<IActionResult> EditPartial(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }
            return PartialView("_EditPartial", recipe);
        }

        // POST: Admin/Recipes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(int id, [Bind("RecipeId,RecipeName,UserId,IsCustom,Restriction,WestEast,Category,DetailedCategory,Steps,Seasoning,Visibility,Photo,Status")] Recipe recipe)
        {
            // 使用 Console.WriteLine 打印 `id` 和 `recipe.RecipeId` 來檢查它們是否一致
            Console.WriteLine("id from route: " + id);
            Console.WriteLine("RecipeId from model: " + recipe.RecipeId);
            if (id != recipe.RecipeId)
            {

                return new JsonResult(new { success = false, error = "食譜ID不符合。" });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recipe);
                    _context.SaveChanges();
                    return new JsonResult(new { success = true });
                }
                catch (DbUpdateConcurrencyException)
                {
                    return new JsonResult(new { success = false, error = "發生並發錯誤。" });
                }
            }

            return new JsonResult(new
            {
                success = false,
                error = "資料無效。",
                errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
            });
        }





        // GET: Admin/Recipes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes
                .FirstOrDefaultAsync(m => m.RecipeId == id);
            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        // POST: Admin/Recipes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe != null)
            {
                _context.Recipes.Remove(recipe);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RecipeExists(int id)
        {
            return _context.Recipes.Any(e => e.RecipeId == id);
        }
    }
}
