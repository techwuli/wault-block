using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace WaultBlock.Models
{
    [Table("ClaimDefinitions")]
    public class ClaimDefinition
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid CredentialSchemaId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public string UserId { get; set; }

        public virtual CredentialSchema CredentialSchema { get; set; }

        public virtual ICollection<UserIndyClaim> UserIndyClaims { get; set; }
    }
}
