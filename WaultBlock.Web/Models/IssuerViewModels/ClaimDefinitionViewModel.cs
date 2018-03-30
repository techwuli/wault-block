using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json.Linq;

namespace WaultBlock.Web.Models.IssuerViewModels
{
    public class ClaimDefinitionViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Fields { get; set; }

        public string UserName { get; set; }

        public string Published { get; set; }

        public string[] FieldArray
        {
            get
            {
                if (string.IsNullOrEmpty(Fields))
                {
                    return new string[0];
                }
                return Fields.Split(',');
            }
            set
            {
                if (value == null)
                {
                    Fields = null;
                }
                else
                {
                    Fields = string.Join(",", value);
                }
            }
        }
    }
}
