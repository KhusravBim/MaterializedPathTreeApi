using System.ComponentModel.DataAnnotations;

namespace MaterializedPathTreeAPI.Models.Info
{
    public class TreeItemInfo
    {
        [Required]
        public Guid Id { get; set; }

        public Guid? ParentId { get; set; }

        [Required]
        public string Value { get; set; }
    }
}