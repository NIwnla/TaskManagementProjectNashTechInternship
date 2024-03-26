using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NProjectMVC.Models;

namespace NProjectMVC.Data
{
    public class NProjectContext : IdentityDbContext<User>
    {
        public NProjectContext(DbContextOptions<NProjectContext> options) : base(options)
        {
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectTask> ProjectTasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>(entity =>
            {
                entity.Property<DateTime>("CreatedTime");
                entity.Property<DateTime>("UpdatedTime");
                entity.HasMany(task => task.Members).WithMany(user => user.Projects);
            });
            modelBuilder.Entity<ProjectTask>(entity =>
            {
                entity.Property<DateTime>("CreatedTime");
                entity.Property<DateTime>("UpdatedTime");
                entity.HasOne(task => task.Project).WithMany(project => project.ProjectTasks).HasForeignKey(task => task.ProjectId);
                entity.HasMany(task => task.AssignedMembers).WithMany(user => user.Tasks);
            });



            base.OnModelCreating(modelBuilder);

        }

        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);


            foreach (var entityEntry in entries)
            {
                if (entityEntry.Properties.Any(e => e.Metadata.Name == "UpdatedTime"))
                {
                    entityEntry.Property("UpdatedTime").CurrentValue = DateTime.Now;

                    if (entityEntry.State == EntityState.Added)
                    {
                        entityEntry.Property("CreatedTime").CurrentValue = DateTime.Now;
                    }
                }
            }
            return base.SaveChanges();
        }

    }


}
