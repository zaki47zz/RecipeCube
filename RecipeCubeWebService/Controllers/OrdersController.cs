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
    public class OrdersController : ControllerBase
    {
        private readonly RecipeCubeContext _context;

        public OrdersController(RecipeCubeContext context)
        {
            _context = context;
        }

        // 創建訂單/訂單明細
        // POST: api/Orders
        [HttpPost("PostOrderOrderItem")]
        public async Task<ActionResult<OrderOrderItemDTO>> PostOrder(OrderOrderItemDTO orderDTO)
        {
            // order實體
            var order = new Order
            {
                OrderId = orderDTO.OrderId,
                UserId = orderDTO.UserId,
                OrderTime = orderDTO.OrderTime,
                TotalAmount = orderDTO.TotalAmount,
                Status = orderDTO.Status,
                OrderAddress = orderDTO.OrderAddress,
                OrderPhone = orderDTO.OrderPhone,
                OrderEmail = orderDTO.OrderEmail,
                OrderRemark = orderDTO.OrderRemark,
                OrderName = orderDTO.OrderName,
            };

            // order新增至Orders表
            _context.Orders.Add(order);

            // 保存 Order
            await _context.SaveChangesAsync();

            // OrderItem實體
            foreach (var orderItemDTO in orderDTO.OrderItemsDTO)
            {
                var orderItem = new OrderItem
                {
                    OrderId= orderItemDTO.OrderId,
                    ProductId = orderItemDTO.ProductId,
                    Quantity = orderItemDTO.Quantity,
                    Price = orderItemDTO.Price,
                };

                // orderItem 新增至 OrderItems
                _context.OrderItems.Add(orderItem);
            }


            try
            {
                // 保存OrderItems
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (OrderExists(order.OrderId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetOrder", new { id = order.OrderId }, orderDTO);
        }

        // 取得我的訂單 join四張表 order orderItem product ingredients
        // GET: api/Orders/5
        [HttpGet("OrderItem/{myUserId}")]
        public async Task<ActionResult<GetOrderOrderItemProduct>> GetOrderByUserId(string myUserId)
        {
            var result = await _context.Orders
                .Where(o => o.UserId == myUserId) //order orderItem 關聯by UserId
                .GroupJoin(
                    _context.OrderItems, // 與訂單明細表連結
                    o => o.OrderId, //訂單的OrderId
                    oi => oi.OrderId,  //訂單明細的OrderId
                    (o, orderItems) => new { o, orderItems } // 返回包含訂單和訂單明細集合的匿名型別
                )
                .SelectMany(                            // 展開orderItems
                    x => x.orderItems.DefaultIfEmpty(), //左外連結訂單明細
                    (x, orderItem) => new { x.o, orderItem } // 返回訂單和訂單明細的匿名型別
                ).GroupJoin(
                    _context.Products,  // 與商品表連結
                    x => x.orderItem.ProductId, // 訂單明細的ProductId
                    p => p.ProductId,  //商品的ProductId
                    (x, products) => new { x.o, x.orderItem, products } // 返回包含訂單、訂單明細和產品集合的匿名型別
                ).SelectMany(         // 展開products
                    x => x.products.DefaultIfEmpty(),  // 左外連結商品
                     (x, product) => new { x.o, x.orderItem, product } // 返回訂單、訂單明細和產品的匿名型別
                ).GroupJoin(
                    _context.Ingredients,  //與食材表連結
                    x => x.product.IngredientId,  // 商品的IngredientId
                    i => i.IngredientId,  // 食材表的IngredientId
                    (x, ingredients) => new { x.o, x.orderItem, x.product, ingredients } // 返回包含訂單、訂單明細、產品和成分集合的匿名型別
                ).SelectMany(
                    x => x.ingredients.DefaultIfEmpty(),  //左外連結食材
                    (x, ingredient) => new GetOrderOrderItemProduct
                    {
                        OrderId = x.o.OrderId,
                        UserId = x.o.UserId,
                        OrderTime = x.o.OrderTime,
                        TotalAmount = x.o.TotalAmount,
                        Status = x.o.Status,
                        OrderAddress = x.o.OrderAddress,
                        OrderPhone = x.o.OrderPhone,
                        OrderEmail = x.o.OrderEmail,
                        OrderRemark = x.o.OrderRemark,
                        OrderName = x.o.OrderName,
                        GetOrderItemProductUnit = x.orderItem == null ? new List<GetOrderItemProductUnit>() : new List<GetOrderItemProductUnit>
                        {
                            new GetOrderItemProductUnit
                            {
                                OrderId = x.orderItem.OrderId,
                                ProductId = x.orderItem.ProductId,
                                Quantity = x.orderItem.Quantity,
                                Price = x.orderItem.Price,
                                ProductName = x.product.ProductName, // 獲取產品名稱
                                IngredientId = x.product.IngredientId, // 獲取食品原料的 IngredientId
                                Photo = x.product.Photo, // 獲取產品照片
                                UnitQuantity = x.product.UnitQuantity,  //獲取單位量
                                Unit = ingredient.Unit // 從 Ingredients 獲取單位
                            }
                        }
                    }
                ).ToListAsync();

            if (result == null || !result.Any())
            {
                return NotFound();
            }

            // 聚合訂單明細到主訂單 DTO 中
            var groupedResult = result
                .GroupBy(o => o.OrderId)
                .Select(g => new GetOrderOrderItemProduct
                {
                    OrderId = g.Key,
                    UserId = g.First().UserId,
                    OrderTime = g.First().OrderTime,
                    TotalAmount = g.First().TotalAmount,
                    Status = g.First().Status,
                    OrderAddress = g.First().OrderAddress,
                    OrderPhone = g.First().OrderPhone,
                    OrderEmail = g.First().OrderEmail,
                    OrderRemark = g.First().OrderRemark,
                    OrderName = g.First().OrderName,
                    GetOrderItemProductUnit = g.SelectMany(x => x.GetOrderItemProductUnit).ToList()
                })
                .ToList();

            return Ok(groupedResult);
        }


        //=========================================================================================

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders.ToListAsync();
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(long id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(long id, Order order)
        {
            if (id != order.OrderId)
            {
                return BadRequest();
            }

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
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

        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(Order order)
        {
            _context.Orders.Add(order);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (OrderExists(order.OrderId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetOrder", new { id = order.OrderId }, order);
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(long id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(long id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
