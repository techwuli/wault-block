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

        public UserIndyClaimStatus Status { get; set; }

        [Required]
        public Guid ClaimDefinitionId { get; set; }

        public string UserId { get; set; }

        public virtual ClaimDefinition ClaimDefinition { get; set; }

        public virtual ApplicationUser User { get; set; }

        public string ClaimRequest { get; set; }

        public string ClaimResponse { get; set; }

        public DateTime TimeCreated { get; set; }

        public DateTime LastUpdated { get; set; }

    }

    public enum UserIndyClaimStatus
    {
        Requested = 0, Confirmed = 1, Canceled = 2
    }
}
