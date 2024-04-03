using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using NProjectMVC.Data;
using NProjectMVC.Models;
using System.Formats.Asn1;
using System.Globalization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace NProjectMVC.Extension
{
	public static class ApplicationDbExtension
	{
		public static IApplicationBuilder SeedData(this IApplicationBuilder app)
		{
			using (var scope = app.ApplicationServices.CreateScope())
			{
				var dbContext = scope.ServiceProvider.GetService<NProjectContext>();
				var userManager = scope.ServiceProvider.GetService<UserManager<User>>();
				dbContext.Database.EnsureCreated();
				if (!dbContext.Projects.Any())
				{
					var projects = new List<Project>();
					string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
					string relativePath = "DataSeed/MOCK_DATA_PROJECT.json";
					string fullPath = Path.Combine(baseDirectory, relativePath);
					if (File.Exists(fullPath))
					{
						using (StreamReader r = new StreamReader(fullPath))
						{
							string json = r.ReadToEnd();
							projects = JsonConvert.DeserializeObject<List<Project>>(json);
						}

						foreach (var p in projects)
						{
							p.Members = new List<User>();
							p.Members.Add(userManager.Users.Where(u => u.UserName == "admin@admin.com").FirstOrDefault());
							dbContext.Projects.Add(p);
						}
						dbContext.SaveChanges();
					}
				}
				if (!dbContext.ProjectTasks.Any())
				{
					var tasks = new List<ProjectTask>();
					var projects = dbContext.Projects.ToList();
					string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
					string relativePath = "DataSeed/MOCK_DATA_TASK.json";
					string fullPath = Path.Combine(baseDirectory, relativePath);
					if (File.Exists(fullPath))
					{
						using (StreamReader r = new StreamReader(fullPath))
						{
							string json = r.ReadToEnd();
							tasks = JsonConvert.DeserializeObject<List<ProjectTask>>(json);
						}

						foreach (var p in projects)
						{
							foreach (var t in tasks)
							{
								t.ProjectId = p.Id;
								t.AssignedMembers = new List<User>();
								t.AssignedMembers.Add(userManager.Users.Where(u => u.UserName == "admin@admin.com").FirstOrDefault());
								dbContext.ProjectTasks.Add(t);
							}
						}
						dbContext.SaveChanges();
					}
				}


				return app;
			}
		}
	}
}
