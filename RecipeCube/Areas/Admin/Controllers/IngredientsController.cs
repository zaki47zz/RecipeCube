using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeCube.Areas.Admin.ViewModels;
using RecipeCube.Models;

namespace RecipeCube.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class IngredientsController : Controller
    {
        private readonly RecipeCubeContext _context;

        public IngredientsController(RecipeCubeContext context)
        {
            _context = context;
        }

        // GET: Admin/Ingredients
        public async Task<IActionResult> IngredientIndexPartial()
        {
            var ingredients = await _context.Ingredients.ToListAsync();

            // 將 Ingredient 列表轉換為 IngredientViewModel 列表
            var viewModel = ingredients.Select(ingredient => new IngredientViewModel
            {
                IngredientId = ingredient.IngredientId,
                IngredientName = ingredient.IngredientName,
                Category = ingredient.Category,
                Synonym = ingredient.Synonym,
                ExpireDay = ingredient.ExpireDay,
                Unit = ingredient.Unit,
                Gram = ingredient.Gram,
                Photo = ingredient.Photo
            }).ToList();

            // 傳遞 ViewModel 列表到 PartialView
            return PartialView("_IngredientIndexPartial", viewModel);
        }

        // GET: Admin/Ingredients/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var ingredient = await _context.Ingredients
        //        .FirstOrDefaultAsync(m => m.IngredientId == id);
        //    if (ingredient == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(ingredient);
        //}

        public async Task<IActionResult> DetailsPartial(int id)
        {
            var ingredient = await _context.Ingredients
                .FirstOrDefaultAsync(m => m.IngredientId == id);

            if (ingredient == null)
            {
                return NotFound();
            }

            var viewmodel = new IngredientViewModel
            {
                IngredientId = ingredient.IngredientId,
                IngredientName = ingredient.IngredientName,
                Category = ingredient.Category,
                Synonym = ingredient.Synonym,
                ExpireDay = ingredient.ExpireDay,
                Unit = ingredient.Unit,
                Gram = ingredient.Gram,
                Photo = ingredient.Photo
            };

            return PartialView("_DetailsPartial", viewmodel);
        }

        // GET: Admin/Ingredients/Create
        public IActionResult CreatePartial()
        {
            var categories = _context.Ingredients.Select(c => c.Category).Distinct().ToList(); //抓資料庫中的categories

            // 利用 Viewbag 傳遞 categories 到 View
            ViewBag.Categories = categories;
            return PartialView("_CreatePartial");
        }

        // POST: Admin/Ingredients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IngredientId,IngredientName,Category,Synonym,ExpireDay,Unit,Gram,Photo")] Ingredient ingredient)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ingredient);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return PartialView("_CreatePartial", ingredient);
        }

        //// GET: Admin/Ingredients/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var ingredient = await _context.Ingredients.FindAsync(id);
        //    if (ingredient == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(ingredient);
        //}

        // GET: Admin/Ingredients/Edit/5
        [HttpGet]
        public async Task<IActionResult> EditPartial(int id)
        {
            // 從資料庫獲取 Ingredient (id == id) 該筆資料
            var ingredient = await _context.Ingredients.FindAsync(id);

            if (ingredient == null)
            {
                return NotFound();
            }

            // 將 Ingredient 資料轉換為 IngredientViewModel 型別
            var viewmodel = new IngredientViewModel
            {
                IngredientId = ingredient.IngredientId,
                IngredientName = ingredient.IngredientName,
                Category = ingredient.Category,
                Synonym = ingredient.Synonym,
                ExpireDay = ingredient.ExpireDay,
                Unit = ingredient.Unit,
                Gram = ingredient.Gram,
                Photo = ingredient.Photo
            };

            return PartialView("_EditPartial", viewmodel);
        }

        // POST: Admin/Ingredients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(int id, [Bind("IngredientId,IngredientName,Category,Synonym,ExpireDay,Unit,Gram,Photo")] Ingredient ingredient)
        {
            if (id != ingredient.IngredientId)
            {
                return new JsonResult(new { success = false, error = "ID不符合!" });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ingredient);
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
        
        // GET: Admin/Ingredients/Delete/5
        public async Task<IActionResult> DeletePartial(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingredient = await _context.Ingredients
                .FirstOrDefaultAsync(m => m.IngredientId == id);
            if (ingredient == null)
            {
                return NotFound();
            }

            var viewmodel = new IngredientViewModel
            {
                IngredientId = ingredient.IngredientId,
                IngredientName = ingredient.IngredientName,
                Category = ingredient.Category,
                Synonym = ingredient.Synonym,
                ExpireDay = ingredient.ExpireDay,
                Unit = ingredient.Unit,
                Gram = ingredient.Gram,
                Photo = ingredient.Photo
            };

            return PartialView("_DeletePartial", viewmodel);
        }

        // POST: Admin/Ingredients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteConfirmed(int id)
        {
            try
            {
                var ingredient =  _context.Ingredients.Find(id);
                if (ingredient != null)
                {
                    _context.Ingredients.Remove(ingredient);
                }
                 _context.SaveChanges();
                return new JsonResult(new { success = true, error = "成功刪除!" });
            }
            catch (DbUpdateConcurrencyException)
            {
                return new JsonResult(new { success = false, error = "發生錯誤!" });
            }
        }

        private bool IngredientExists(int id)
        {
            return _context.Ingredients.Any(e => e.IngredientId == id);
        }
    }
}
