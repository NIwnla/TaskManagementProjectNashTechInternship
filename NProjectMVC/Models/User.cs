using Microsoft.AspNetCore.Identity;

namespace NProjectMVC.Models
{
	public class User: IdentityUser
	{
		[PersonalData]
		public string? Name { get; set; }
		[PersonalData]
		public DateTime? DOB {  get; set; }
		[PersonalData]
		public string? ProfilePic { get; set; }
		public virtual ICollection<ProjectTask>? Tasks { get; set; }
		public virtual ICollection<Project>? Projects { get; set; }
    }
}
