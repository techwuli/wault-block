using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WaultBlock.Models
{
    [Table("UserIndyClaims")]
    public class UserIndyClaim
    {
        [Key]
        public Guid Id { get; set; }

        public bool Issued { get; set; }

        [Required]
        public Guid ClaimDefinitionId { get; set; }

        public string UserId { get; set; }

        public virtual ClaimDefinition ClaimDefinition { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
