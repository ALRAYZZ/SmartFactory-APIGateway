namespace MachinesApi.Models
{
	public class MachineModel
	{
		public int Id { get; set; }
		public string? Name { get; set; }
		public double Temperature { get; set; }
		public bool IsRunning { get; set; }
	}
}
