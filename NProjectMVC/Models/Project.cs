using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NProjectMVC.Models
{
    [Table("Projects")]
    public class Project
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public DateTime? Deadline { get; set; }
        public float? EstimatedWorkTime { get; set; }

        public float? TimeSpent { get; set; }

        public virtual ICollection<ProjectTask>? ProjectTasks { get; set; }
        public virtual ICollection<User>? Members { get; set; }
    }
}
