using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrdersApi.Models;

namespace OrdersApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
		// Hardcoded list of orders for demonstration purposes
		private static readonly List<OrdersModel> _orders = new()
        {
            new OrdersModel { Id = 1, Item = "Laptop", Quantity = 20 },
			new OrdersModel { Id = 2, Item = "Smartphone", Quantity = 50 },
		};

        [HttpGet]
        public ActionResult<IEnumerable<OrdersModel>> GetOrders()
		{
			return Ok(_orders);
		}

		[HttpPost]
		public ActionResult<OrdersModel> Post([FromBody] OrdersModel order)
		{
			order.Id = _orders.Count + 1; // Simple for testing purposes aware of duplication issues if deletes happen
			_orders.Add(order);
			return CreatedAtAction(nameof(GetOrders), new { id = order.Id }, order);
		}
	}
}
