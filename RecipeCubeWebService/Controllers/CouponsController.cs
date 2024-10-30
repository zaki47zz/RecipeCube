using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeCubeWebService.DTO;
using RecipeCubeWebService.Models;

namespace RecipeCubeWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponsController : ControllerBase
    {
        private readonly RecipeCubeContext _context;

        public CouponsController(RecipeCubeContext context)
        {
            _context = context;
        }

        // 獲取所有優惠券配user_coupons
        // GET: api/Coupons
        [HttpGet("GetCouponsWithUserCoupons")]
        public async Task<ActionResult<IEnumerable<CouponsDTO>>> GetCouponsWithUserCoupons()
        {
            var coupons = await (
                from coupon in _context.Coupons
                join userCoupon in _context.UserCoupons
                    on coupon.CouponId equals userCoupon.CouponId
                select new CouponsDTO
                    {
                        CouponId = coupon.CouponId,
                        CouponName = coupon.CouponName,
                        CouponStatus = coupon.Status,
                        DiscountType = coupon.DiscountType,
                        DiscountValue = coupon.DiscountValue,
                        UserConponId = userCoupon.UserConponId,
                        UserId = userCoupon.UserId,
                        UsedStatus = userCoupon.Status,
                        AcquireDate = userCoupon.AcquireDate,
                        MinSpend = coupon.MinSpend,
                    }
                ).ToListAsync();
            return Ok( coupons );
        }


        // GET: api/Coupons
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Coupon>>> GetCoupons()
        {
            return await _context.Coupons.ToListAsync();
        }

        // GET: api/Coupons/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Coupon>> GetCoupon(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);

            if (coupon == null)
            {
                return NotFound();
            }

            return coupon;
        }

        // PUT: api/Coupons/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCoupon(int id, Coupon coupon)
        {
            if (id != coupon.CouponId)
            {
                return BadRequest();
            }

            _context.Entry(coupon).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CouponExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Coupons
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Coupon>> PostCoupon(Coupon coupon)
        {
            _context.Coupons.Add(coupon);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCoupon", new { id = coupon.CouponId }, coupon);
        }

        // DELETE: api/Coupons/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCoupon(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null)
            {
                return NotFound();
            }

            _context.Coupons.Remove(coupon);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CouponExists(int id)
        {
            return _context.Coupons.Any(e => e.CouponId == id);
        }
    }
}
