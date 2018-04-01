using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WaultBlock.Models
{
    [Table("WalletRecords")]
    public class WalletRecord
    {
        [Key]
        public string Key { get; set; }

        [Required]
        public string WalletName { get; set; }

        [Required]
        public string UserId { get; set; }

        public string Value { get; set; }

        [Required]
        public DateTime TimeCreated { get; set; }

        public virtual WalletData WalletData { get; set; }
    }
}
