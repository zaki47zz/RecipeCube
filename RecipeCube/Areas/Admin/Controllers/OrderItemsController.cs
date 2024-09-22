using MailKit.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeCube.Areas.Admin.ViewModels;
using RecipeCube.Models;
using System.Diagnostics;

namespace RecipeCube.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderItemsController : Controller
    {
        private readonly RecipeCubeContext _context;

        public OrderItemsController(RecipeCubeContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OrderItemIndexPartial(long orderid)
        {
            ViewBag.OrderId = orderid;
            Debug.WriteLine($"IIIIIIDDDDDDD==================={orderid}");
            var orderItems = await _context.OrderItems.ToListAsync();
            var viewModel = orderItems.Select(orderItem => new OrderItemViewModel
            {
                OrderItemId=orderItem.OrderItemId,
                OrderId=orderItem.OrderId,
                ProductId=orderItem.ProductId,
                Quantity=orderItem.Quantity,
                Price=orderItem.Price,
            }).ToList();
            return PartialView("_OrderItemIndexPartial", viewModel);
        }

        // GET: Admin/OrderItems/Create
        public IActionResult CreatePartial()
        {
            return PartialView("_CreatePartial");
        }

        // POST: Admin/OrderItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderItemId,OrderId,ProductId,Quantity,Price")] OrderItem orderItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderItem);
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
                return Json(new { success = true });
            }
            return PartialView("_CreatePartial", orderItem);
        }

        // GET: Admin/OrderItems/EditPartial/5
        public async Task<IActionResult> EditPartial(int id)
        {
            var orderItem = await _context.OrderItems.FindAsync(id);
            if (orderItem == null)
            {
                return NotFound();
            }
            return PartialView("_EditPartial",orderItem);
        }

        // POST: Admin/OrderItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(int id, [Bind("OrderItemId,OrderId,ProductId,Quantity,Price")] OrderItem orderItem)
        {
            if (id != orderItem.OrderItemId)
            {
                return new JsonResult(new { success = false, error = "ID不符合!" });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderItem);
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

        // GET: Admin/OrderItems/Delete/5
        public async Task<IActionResult> DeletePartial(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderItem = await _context.OrderItems
                .FirstOrDefaultAsync(m => m.OrderItemId == id);
            if (orderItem == null)
            {
                return NotFound();
            }

            return PartialView("_DeletePartial",orderItem);
        }

        // POST: Admin/OrderItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteConfirmed(int id)
        {
            try { 
            var orderItem = _context.OrderItems.Find(id);
            if (orderItem != null)
            {
                _context.OrderItems.Remove(orderItem);
            }

            _context.SaveChanges();
            return new JsonResult(new { success = true, error = "成功刪除!" });
            }
            catch (DbUpdateConcurrencyException)
            {
                return new JsonResult(new { success = false, error = "發生錯誤!" });
            }
        }

        ////GET: Admin/OrderItems
        //public async Task<IActionResult> Index(long orderid)
        //{
        //    ViewBag.OrderId = orderid;
        //    return View(await _context.OrderItems.ToListAsync());
        //}




        // GET: Admin/OrderItems/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var orderItem = await _context.OrderItems
        //        .FirstOrDefaultAsync(m => m.OrderItemId == id);
        //    if (orderItem == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(orderItem);
        //}

        //// GET: Admin/OrderItems/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Admin/OrderItems/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("OrderItemId,OrderId,ProductId,Quantity,Price")] OrderItem orderItem)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(orderItem);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(orderItem);
        //}

        //// GET: Admin/OrderItems/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var orderItem = await _context.OrderItems.FindAsync(id);
        //    if (orderItem == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(orderItem);
        //}

        //// POST: Admin/OrderItems/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("OrderItemId,OrderId,ProductId,Quantity,Price")] OrderItem orderItem)
        //{
        //    if (id != orderItem.OrderItemId)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(orderItem);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!OrderItemExists(orderItem.OrderItemId))
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
        //    return View(orderItem);
        //}

        //// GET: Admin/OrderItems/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var orderItem = await _context.OrderItems
        //        .FirstOrDefaultAsync(m => m.OrderItemId == id);
        //    if (orderItem == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(orderItem);
        //}

        //// POST: Admin/OrderItems/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var orderItem = await _context.OrderItems.FindAsync(id);
        //    if (orderItem != null)
        //    {
        //        _context.OrderItems.Remove(orderItem);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool OrderItemExists(int id)
        {
            return _context.OrderItems.Any(e => e.OrderItemId == id);
        }
    }
}
