namespace NProjectMVC.Models.ViewModel
{
	public class ProjectVM
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public DateTime? Deadline { get; set; }
		public float? EstimatedWorkTime { get; set; }

		public float? TimeSpent { get; set; }
		public int? TotalTask { get; set; }
		public int? CompletedTask { get; set; }
	}
}
