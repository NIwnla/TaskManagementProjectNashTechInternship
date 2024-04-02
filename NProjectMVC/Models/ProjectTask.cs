using NProjectMVC.Models.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NProjectMVC.Models
{
    [Table("ProjectTasks")]
    public class ProjectTask
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        public Guid ProjectId { get; set; }
        [Required]
        [StringLength(200)]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public DateTime? Deadline {  get; set; }
        [Required]
        public int Status { get; set; }
        public float? EstimatedWorkTime { get; set; }
        public float? TimeSpent { get; set; }
        public virtual Project? Project { get; set; }
        public virtual ICollection<User>? AssignedMembers { get; set; }
        public virtual ICollection<WorkedTask>? WorkedTasks { get; set; }
    }
}
