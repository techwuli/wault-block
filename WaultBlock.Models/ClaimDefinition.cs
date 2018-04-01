using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace WaultBlock.Models
{
    [Table("ClaimDefinitions")]
    public class ClaimDefinition
    {
        [NotMapped]
        public string[] FieldArray
        {
            get
            {
                return string.IsNullOrEmpty(Fields) ? null : JsonConvert.DeserializeObject<string[]>(Fields);
            }
            set
            {
                Fields = value == null ? "[]" : JsonConvert.SerializeObject(value);
            }
        }

        [Required]
        public string Fields { get; set; }

        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [NotMapped]
        public string[] ProofFieldArray
        {
            get
            {
                return string.IsNullOrEmpty(ProofFields) ? null : JsonConvert.DeserializeObject<string[]>(ProofFields);
            }
            set
            {
                ProofFields = value == null ? "[]" : JsonConvert.SerializeObject(value);
            }
        }

        public string ProofFields { get; set; }

        [Required]
        [DefaultValue(false)]
        public bool Published { get; set; }

        public virtual ApplicationUser User { get; set; }

        [Required]
        public string UserId { get; set; }
    }
}
