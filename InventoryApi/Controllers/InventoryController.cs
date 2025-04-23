using InventoryApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
	public class InventoryController : ControllerBase
    {
        private static readonly List<ItemModel> _inventory = new()
        {
            new ItemModel { Item = "Item1", Stock = 10 },
            new ItemModel { Item = "Item2", Stock = 20 },
        };

        [HttpGet]
        public ActionResult<IEnumerable<ItemModel>> GetInventory()
        {
            return Ok(_inventory);
        }

        [HttpPut]
        public ActionResult Update([FromBody] ItemModel updatedItem)
        {
            var item = _inventory.FirstOrDefault(i => i.Item == updatedItem.Item);
            if (item == null)
            {
                return NotFound($"Item {updatedItem.Item} not found.");
			}
            item.Stock = updatedItem.Stock;
            return Ok($"Item {updatedItem.Item} updated to {updatedItem.Stock}.");
		}
	}
}
