using NProjectMVC.Models.Enum;

namespace NProjectMVC.Models.ViewModel
{
	public class UserVM
	{
		public string? Id { get; set; }
		public string? Name { get; set; }
		public DateTime? DOB { get; set; }
		public string? Email { get; set; }
		public string? UserName {  get; set; }
		public UserRoles Role {  get; set; }
	}
}
