using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using WaultBlock.Models;

namespace WaultBlock.Web.Models.IssuerViewModels
{
    public class ClaimDefinitionViewModel
    {
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

        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string[] ProofFieldArrays
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

        public bool Published { get; set; }

        public string UserName { get; set; }

        public static ClaimDefinitionViewModel Create(ClaimDefinition claimDefinition)
        {
            var model = new ClaimDefinitionViewModel
            {
                Id = claimDefinition.Id,
                Fields = claimDefinition.Fields,
                Name = claimDefinition.Name,
                UserName = claimDefinition.User.UserName,
                Published = claimDefinition.Published
            };

            return model;
        }

    }
}
