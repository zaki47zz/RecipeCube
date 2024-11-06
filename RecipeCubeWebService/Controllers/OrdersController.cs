using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        //==========================================================================================
        

        string ServerReturnUrl = "https://d3e3-2001-b400-e2d0-410d-1968-d4ef-34de-c9b0.ngrok-free.app";

        string ClientReturnUrl = "https://485e-59-125-142-166.ngrok-free.app";

        // 支付請求
        [HttpPost("StartPayment")]
        public ActionResult StartPayment([FromBody] Order order)
        {
            var paymentHtml = CreatePaymentRequest(order);
            return Content(paymentHtml, "text/html");  // 返回 HTML 給前端
        }

        // 建立支付請求 (生成 HTML 表單)
        private string CreatePaymentRequest(Order order)
        {
            var paymentData = new Dictionary<string, string>
       {
           { "MerchantID", "3002607" },  // 特店編號
           { "MerchantTradeNo", order.OrderId.ToString() },  // 特店訂單編號
           { "MerchantTradeDate", DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss") },  // 特店交易時間
           { "PaymentType", "aio" },  // 交易類型  固定 "aio"
           { "TotalAmount", order.TotalAmount.ToString() },  // 交易金額 僅能整數
           { "TradeDesc", "訂單支付" },  // 交易描述
           { "ItemName", "食品food" },  // 商品名稱
           { "ReturnURL", $"{ServerReturnUrl}/api/Orders/PayInfo" },  // server 端通知網址
           { "ClientBackURL", $"{ClientReturnUrl}/storeproduct" },  // 導回 client 頁面
           { "ChoosePayment", "Credit" },  // 預設付款方式
           { "EncryptType", "1" }  // CheckMacValue 加密類型 固定 "1" SHA256加密
       };

            paymentData.Add("CheckMacValue", GetCheckMacValue(paymentData));  // 檢查碼

            // 生成 HTML 表單
            var formHtml = new StringBuilder();
            formHtml.AppendLine("<form id='ecpay-form' action='https://payment-stage.ecpay.com.tw/Cashier/AioCheckOut/V5' method='POST'>");

            foreach (var keyValuePair in paymentData)
            {
                formHtml.AppendLine($"<input type='hidden' name='{keyValuePair.Key}' value='{keyValuePair.Value}' />");
            }

            formHtml.AppendLine("</form>");
            formHtml.AppendLine("<script>document.getElementById('ecpay-form').submit();</script>");

            return formHtml.ToString();  // 返回完整的 HTML 表單
        }

        // 生成 CheckMacValue 的方法
        private string GetCheckMacValue(Dictionary<string, string> order)
        {
            var param = order.Keys.OrderBy(x => x).Select(key => key + "=" + order[key]).ToList();
            var checkValue = string.Join("&", param);

            // 測試用的 HashKey 和 HashIV
            var hashKey = "pwFHCqoQZGmho4w6";
            var hashIV = "EkRm7iFT261dpevs";

            checkValue = $"HashKey={hashKey}&{checkValue}&HashIV={hashIV}";
            checkValue = HttpUtility.UrlEncode(checkValue).ToLower();
            checkValue = GetSHA256(checkValue);

            return checkValue.ToUpper();
        }

        // SHA256 加密
        private string GetSHA256(string value)
        {
            var result = new StringBuilder();
            var sha256 = SHA256.Create();
            var bts = Encoding.UTF8.GetBytes(value);
            var hash = sha256.ComputeHash(bts);
            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }
            return result.ToString();
        }
        //================================================================================================================

        // 接收綠界回傳付款是否成功
        [HttpPost("PayInfo")]
        public async Task<ActionResult> PayInfo([FromForm] PaymentResponse paymentResponse)
        {
            // 檢查 paymentResponse 是否為 null
            if (paymentResponse == null)
            {
                return BadRequest("Invalid payment information.");
            }

            // 查詢訂單
            var order = await _context.Orders
                .FirstOrDefaultAsync(m => m.OrderId.ToString() == paymentResponse.MerchantTradeNo);
            
            // 查詢商品 

            if (order != null)
            {
                // 如果付款成功，更新 status 為 2
                if (paymentResponse.RtnCode == 1) // 根據您的業務邏輯判斷成功的條件
                {
                    order.Status = 2; // 將 status 更新為 2

                    // 查詢訂單明細
                    var orderItems = await _context.OrderItems
                        .Where(oi => oi.OrderId == order.OrderId)
                        .ToListAsync();

                    // 對應商品
                    foreach (var items in orderItems)
                    {
                        var product = await _context.Products
                            .FirstOrDefaultAsync(p => p.ProductId == items.ProductId);
                            
                        if(product != null)
                        {
                            // 更改庫存
                            product.Stock -= Convert.ToInt32(product.UnitQuantity) * items.Quantity;

                            // 如果扣完庫存小於 0 
                            if (product.Stock < 0)
                            {
                                product.Stock = 0;
                            }
                        }
                    }
                }

                await _context.SaveChangesAsync(); // 保存變更
            }

            return Ok("OK");
        }

        // 專給繼續付款用的綠界api===========================================================================
        // 支付請求
        [HttpPost("StartPaymentForContinue")]
        public ActionResult StartPaymentForContinue([FromBody] Order order)
        {
            var paymentHtml = CreatePaymentRequestForContinue(order);
            return Content(paymentHtml, "text/html");  // 返回 HTML 給前端
        }

        // 建立支付請求 (生成 HTML 表單)
        private string CreatePaymentRequestForContinue(Order order)
        {
            var paymentData = new Dictionary<string, string>
       {
           { "MerchantID", "3002607" },  // 特店編號
           { "MerchantTradeNo", order.OrderId.ToString() },  // 特店訂單編號
           { "MerchantTradeDate", DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss") },  // 特店交易時間
           { "PaymentType", "aio" },  // 交易類型  固定 "aio"
           { "TotalAmount", order.TotalAmount.ToString() },  // 交易金額 僅能整數
           { "TradeDesc", "訂單支付" },  // 交易描述
           { "ItemName", "食品food" },  // 商品名稱
           { "ReturnURL", $"{ServerReturnUrl}/api/Orders/PayInfoForContinue" },  // server 端通知網址
           { "ClientBackURL", $"{ClientReturnUrl}/storeproduct" },  // 導回 client 頁面
           { "ChoosePayment", "Credit" },  // 預設付款方式
           { "EncryptType", "1" }  // CheckMacValue 加密類型 固定 "1" SHA256加密
       };

            paymentData.Add("CheckMacValue", GetCheckMacValueForContinue(paymentData));  // 檢查碼

            // 生成 HTML 表單
            var formHtml = new StringBuilder();
            formHtml.AppendLine("<form id='ecpay-form' action='https://payment-stage.ecpay.com.tw/Cashier/AioCheckOut/V5' method='POST'>");

            foreach (var keyValuePair in paymentData)
            {
                formHtml.AppendLine($"<input type='hidden' name='{keyValuePair.Key}' value='{keyValuePair.Value}' />");
            }

            formHtml.AppendLine("</form>");
            formHtml.AppendLine("<script>document.getElementById('ecpay-form').submit();</script>");

            return formHtml.ToString();  // 返回完整的 HTML 表單
        }

        // 生成 CheckMacValue 的方法
        private string GetCheckMacValueForContinue(Dictionary<string, string> order)
        {
            var param = order.Keys.OrderBy(x => x).Select(key => key + "=" + order[key]).ToList();
            var checkValue = string.Join("&", param);

            // 測試用的 HashKey 和 HashIV
            var hashKey = "pwFHCqoQZGmho4w6";
            var hashIV = "EkRm7iFT261dpevs";

            checkValue = $"HashKey={hashKey}&{checkValue}&HashIV={hashIV}";
            checkValue = HttpUtility.UrlEncode(checkValue).ToLower();
            checkValue = GetSHA256ForContinue(checkValue);

            return checkValue.ToUpper();
        }

        // SHA256 加密
        private string GetSHA256ForContinue(string value)
        {
            var result = new StringBuilder();
            var sha256 = SHA256.Create();
            var bts = Encoding.UTF8.GetBytes(value);
            var hash = sha256.ComputeHash(bts);
            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }
            return result.ToString();
        }
        //================================================================================================================

        // 接收綠界回傳付款是否成功
        [HttpPost("PayInfoForContinue")]
        public async Task<ActionResult> PayInfoForContinue([FromForm] PaymentResponse paymentResponse)
        {
            // 檢查 paymentResponse 是否為 null
            if (paymentResponse == null)
            {
                return BadRequest("Invalid payment information.");
            }


            string merchantTradeNo = paymentResponse.MerchantTradeNo;

            // 確保商戶交易號有足夠長度以移除 "00"
            if (merchantTradeNo.Length < 2 || !merchantTradeNo.EndsWith("00"))
            {
                return BadRequest("Invalid order ID format.");
            }

            // 移除尾部的 "00" 以獲取原始訂單編號
            string originalOrderIdString = merchantTradeNo.Substring(0, merchantTradeNo.Length - 2);

            // 查詢訂單
            var order = await _context.Orders
                .FirstOrDefaultAsync(m => m.OrderId.ToString() == originalOrderIdString.ToString()); 

            // 查詢商品 

            if (order != null)
            {
                // 如果付款成功，更新 status 為 2
                if (paymentResponse.RtnCode == 1) // 根據您的業務邏輯判斷成功的條件
                {
                    order.Status = 2; // 將 status 更新為 2

                    // 查詢訂單明細
                    var orderItems = await _context.OrderItems
                        .Where(oi => oi.OrderId == order.OrderId)
                        .ToListAsync();

                    // 對應商品
                    foreach (var items in orderItems)
                    {
                        var product = await _context.Products
                            .FirstOrDefaultAsync(p => p.ProductId == items.ProductId);

                        if (product != null)
                        {
                            // 更改庫存
                            product.Stock -= Convert.ToInt32(product.UnitQuantity) * items.Quantity;

                            // 如果扣完庫存小於 0 
                            if (product.Stock < 0)
                            {
                                product.Stock = 0;
                            }
                        }
                    }
                }

                await _context.SaveChangesAsync(); // 保存變更
            }

            return Ok("OK");
        }


        //================================================================================================================

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
