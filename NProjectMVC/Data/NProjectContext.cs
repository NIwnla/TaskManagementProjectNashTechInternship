﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NProjectMVC.Models;
using NProjectMVC.Models.Enum;

namespace NProjectMVC.Data
{
    public class NProjectContext : IdentityDbContext<User>
    {
        public NProjectContext(DbContextOptions<NProjectContext> options) : base(options)
        {
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectTask> ProjectTasks { get; set; }
        public DbSet<WorkedTask> WorkedTasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>(entity =>
            {
                entity.Property<DateTime>("CreatedTime");
                entity.Property<DateTime>("UpdatedTime");
                entity.Property<string>("CreatedBy");
                entity.Property<string>("UpdatedBy");
                entity.HasMany(task => task.Members).WithMany(user => user.Projects);
            });
            modelBuilder.Entity<ProjectTask>(entity =>
            {
                entity.Property<DateTime>("CreatedTime");
                entity.Property<DateTime>("UpdatedTime");
                entity.Property<string>("CreatedBy");
                entity.Property<string>("UpdatedBy");
                entity.HasOne(task => task.Project).WithMany(project => project.ProjectTasks).HasForeignKey(task => task.ProjectId);
                entity.HasMany(task => task.AssignedMembers).WithMany(user => user.Tasks);
            });
            modelBuilder.Entity<WorkedTask>(entity =>
            {
                entity.Property<DateTime>("CreatedTime");
                entity.Property<DateTime>("UpdatedTime");
                entity.Property<string>("CreatedBy");
                entity.Property<string>("UpdatedBy");
                entity.HasOne(work => work.ProjectTask).WithMany(task => task.WorkedTasks).HasForeignKey(work => work.TaskId);
                entity.HasOne(work => work.User).WithMany(user => user.WorkedTasks).HasForeignKey(work => work.UserId);
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

        public int SaveChanges(string userId)
        {
            var entries = ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);
            foreach (var entityEntry in entries)
            {
                if (entityEntry.Properties.Any(e => e.Metadata.Name == "UpdatedBy"))
                {
                    entityEntry.Property("UpdatedBy").CurrentValue = userId;
                    if (entityEntry.State == EntityState.Added)
                    {
                        entityEntry.Property("CreatedBy").CurrentValue = userId;
                    }
                }
            }
            return SaveChanges();
        }

        public T GetShadowProperty<T>(string shadowPropertyName, object entity)
        {
			var entry = Entry(entity);
            T value = default(T);
            if (entry != null)
            {
                if (entry.Properties.Any(e => e.Metadata.Name == shadowPropertyName))
                {
                    value =(T) entry.Property(shadowPropertyName).CurrentValue;
                }
            }
            return value;
        }



    }


}
