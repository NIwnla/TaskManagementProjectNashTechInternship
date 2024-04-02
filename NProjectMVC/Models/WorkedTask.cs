using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NProjectMVC.Models
{
    [Table("WorkedTasks")]
    public class WorkedTask
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public Guid TaskId { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public string Description {  get; set; }

        public virtual ProjectTask? ProjectTask { get; set; }
        public virtual User? User { get; set; }
    }
}
