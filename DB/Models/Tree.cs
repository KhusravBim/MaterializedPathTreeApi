using System.ComponentModel.DataAnnotations;

namespace MaterializedPathTreeAPI.DB.Models
{
    public class Tree
    {
        [Key]
        [StringLength(50, MinimumLength = 3)]
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(5)]
        public string Sorting { get; set; }
    }
}