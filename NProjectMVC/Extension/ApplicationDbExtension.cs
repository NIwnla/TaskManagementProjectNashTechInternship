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
                dbContext.Database.EnsureCreated();
                if (!dbContext.Projects.Any())
                {
                    var projects = new List<Project>();
                    using (StreamReader r = new StreamReader("D:/VSProject/NProject/NProjectMVC/DataSeed/MOCK_DATA_PROJECT.json"))
                    {
                        string json = r.ReadToEnd();
                        projects = JsonConvert.DeserializeObject<List<Project>>(json);
                    }

                    foreach (var p in projects)
                        dbContext.Projects.Add(p);

                    dbContext.SaveChanges();
                }
                if (!dbContext.ProjectTasks.Any())
                {
                    var tasks = new List<ProjectTask>();
                    var projects = dbContext.Projects.ToList();
                    using (StreamReader r = new StreamReader("D:/VSProject/NProject/NProjectMVC/DataSeed/MOCK_DATA_TASK.json"))
                    {
                        string json = r.ReadToEnd();
                        tasks = JsonConvert.DeserializeObject<List<ProjectTask>>(json);
                    }

                    foreach (var p in projects)
                    {
                        foreach (var t in tasks)
                        {
                            t.ProjectId = p.Id;
                            dbContext.ProjectTasks.Add(t);
                        }
                    }
                    dbContext.SaveChanges();
                }


                return app;
            }
        }
    }
}
