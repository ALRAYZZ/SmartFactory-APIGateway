using MachinesApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MachinesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
	public class MachineController : ControllerBase
    {
        private static readonly List<MachineModel> _machines = new()
        {
            new MachineModel { Id = 1, Name = "Machine A", Temperature = 75.5, IsRunning = true },
			new MachineModel { Id = 2, Name = "Machine B", Temperature = 80.0, IsRunning = false },
		};

		[HttpGet("status")]
        public ActionResult<IEnumerable<MachineModel>> GetStatus()
        {
			return Ok(_machines);
		}

	}
}
