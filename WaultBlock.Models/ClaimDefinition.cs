using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WaultBlock.Models
{
    [Table("ClaimDefinitions")]
    public class ClaimDefinition
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        [DefaultValue(false)]
        public bool Published { get; set; }

        [Required]
        public string Name { get; set; }

        public string Fields { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
