using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace WaultBlock.Models
{
    [Table("CredentialSchemas")]
    public class CredentialSchema
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Version { get; set; }

        [Required]
        public string Attributes { get; set; }

        public string UserId { get; set; }

        [NotMapped]
        public string[] AttributeArray
        {
            get
            {
                return string.IsNullOrEmpty(Attributes) ? new string[0] : JsonConvert.DeserializeObject<string[]>(Attributes);
            }

            set
            {

                Attributes = value == null ? "[]" : JsonConvert.SerializeObject(value);
            }
        }

        public virtual ICollection<ClaimDefinition> ClaimDefinitions { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
