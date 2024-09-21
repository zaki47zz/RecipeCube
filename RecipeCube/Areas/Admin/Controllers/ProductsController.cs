using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeCube.Areas.Admin.ViewModels;
using RecipeCube.Models;

namespace RecipeCube.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly RecipeCubeContext _context;

        public ProductsController(RecipeCubeContext context)
        {
            _context = context;
        }

        // GET: Admin/Products/ProductIndexPartial
        public async Task<IActionResult> ProductIndexPartial()
        {
            var products = await _context.Products.ToListAsync();

            // 將 Product 列表轉換為 ProductViewModel 列表
            var viewModel = products.Select(product => new ProductViewModel
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                IngredientId = product.IngredientId,
                Price = product.Price,
                Stock = product.Stock,
                Status = product.Status,
                Photo = product.Photo,
            }).ToList();

            // 傳遞 ViewModle 列表到 PartialView
            return PartialView("_ProductIndexPartial", viewModel);
        }

        // GET: Admin/Products/DetailsPartial
        public async Task<IActionResult> DetailsPartial(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return PartialView("_DetailsPartial",product);
        }

        // GET: Admin/Products/CreatePartial
        public IActionResult CreatePartial ()
        {
            return PartialView("_CreatePartial");
        }

        // POST: Admin/Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("ProductId,ProductName,IngredientId,Price,Stock,Status,Photo")] Product product
        )
        {
            if (ModelState.IsValid)
            {
                //將上傳的圖片寫進資料庫
                if (Request.Form.Files["Photo"] != null)
                {
                    var file = Request.Form.Files["Photo"];
                    var fileName = Path.GetFileName(file.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "ingredient", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // 寫產品圖片檔名
                    product.Photo = fileName;
                }
                //=====================================================================
                _context.Add(product);
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
                return Json(new { success = true });
            }
            return PartialView("_CreatePartial", product);
        }

        // Get:Admin/Products/EditPartial
        public async Task<IActionResult> EditPartial(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return PartialView("_EditPartial",product);
        }

        // POST:Admin/Products/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(
            int id,
            [Bind("ProductId,ProductName,IngredientId,Price,Stock,Status,Photo")] Product product
        )
        {
            if (id != product.ProductId)
            {
                return new JsonResult(new { success = false, error = "ID不符合" });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //將上傳修改的圖片寫進資料庫
                    Product p = _context.Products.Find(product.ProductId);
                    if (Request.Form.Files["Photo"] != null)
                    {
                        var file = Request.Form.Files["Photo"];
                        var fileName = Path.GetFileName(file.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "ingredient", fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        // 更新產品圖片
                        product.Photo = fileName;
                    }
                    else
                    {
                        product.Photo = p.Photo;
                    }
                    _context.Entry(p).State = EntityState.Detached;

                    //=====================================================================

                    _context.Update(product);
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

        // Get: /Products/ShowPhotoPartial
        public async Task<IActionResult> ShowPhotoPartial(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return PartialView("_ShowPhotoPartial", product);
        }

        // GET: Admin/Products
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }

        //GET: Admin/Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        //// GET: Admin/Products/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        // POST: Admin/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(
        //    [Bind("ProductId,ProductName,IngredientId,Price,Stock,Status,Photo")] Product product
        //)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        //將上傳的圖片寫進資料庫
        //        if (Request.Form.Files["Photo"] != null)
        //        {
        //            var file = Request.Form.Files["Photo"];
        //            var fileName = Path.GetFileName(file.FileName);
        //            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "ingredient", fileName);

        //            using (var stream = new FileStream(filePath, FileMode.Create))
        //            {
        //                await file.CopyToAsync(stream);
        //            }

        //            // 寫產品圖片檔名
        //            product.Photo = fileName;
        //        }
        //        //=====================================================================
        //        _context.Add(product);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(product);
        //}

        //GET: Admin/Products/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var product = await _context.Products.FindAsync(id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(product);
        //}

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(
        //    int id,
        //    [Bind("ProductId,ProductName,IngredientId,Price,Stock,Status,Photo")] Product product
        //)
        //{
        //    if (id != product.ProductId)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            //將上傳修改的圖片寫進資料庫
        //            Product p = await _context.Products.FindAsync(product.ProductId);
        //            if (Request.Form.Files["Photo"] != null)
        //            {
        //                var file = Request.Form.Files["Photo"];
        //                var fileName = Path.GetFileName(file.FileName);
        //                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "ingredient", fileName);

        //                using (var stream = new FileStream(filePath, FileMode.Create))
        //                {
        //                    await file.CopyToAsync(stream);
        //                }

        //                // 更新產品圖片
        //                product.Photo = fileName;
        //            }
        //            else
        //            {
        //                product.Photo = p.Photo;
        //            }
        //            _context.Entry(p).State = EntityState.Detached;

        //            //=====================================================================

        //            _context.Update(product);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!ProductExists(product.ProductId))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }

        //    return View(product);
        //}

        // GET: Admin/Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
