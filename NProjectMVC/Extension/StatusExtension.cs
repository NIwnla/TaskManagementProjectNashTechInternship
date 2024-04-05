using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NProjectMVC.Models.Enum;

namespace NProjectMVC.Extension
{
	public static class StatusExtension
	{
		public static SelectList CreateSelectList(this Status status)
		{
			var selectItems = new List<SelectListItem>();
			selectItems.Add(new SelectListItem { Text = Status.Complete.ToString(), Value = Convert.ToString((int)Status.Complete) });
			selectItems.Add(new SelectListItem { Text = Status.Canceled.ToString(), Value = Convert.ToString((int)Status.Canceled) });
			selectItems.Add(new SelectListItem { Text = Status.InProgress.ToString(), Value = Convert.ToString((int)Status.InProgress) });
			selectItems.Add(new SelectListItem { Text = Status.OnHold.ToString(), Value = Convert.ToString((int)Status.OnHold) });
			selectItems.Add(new SelectListItem { Text = Status.NotStarted.ToString(), Value = Convert.ToString((int)Status.NotStarted) });
			return new SelectList(selectItems);
		}
	}
}
