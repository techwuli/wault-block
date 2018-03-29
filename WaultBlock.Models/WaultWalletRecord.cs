using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WaultBlock.Models
{
    [Table("WaultWalletRecords")]
    public class WaultWalletRecord
    {
        [Key]
        public string Key { get; set; }

        [Required]
        public string WaultWalletName { get; set; }

        [Required]
        public string UserId { get; set; }

        public string Value { get; set; }

        [Required]
        public DateTime TimeCreated { get; set; }

        public virtual WaultWallet WaultWallet { get; set; }
    }
}
