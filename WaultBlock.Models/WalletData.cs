using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WaultBlock.Models
{
    [Table("WalletDatas")]
    public class WalletData
    {
        [Key, Column(Order = 0)]
        public string Name { get; set; }

        public DateTime TimeCreated { get; set; }

        public string Did { get; set; }

        public string VerKey { get; set; }

        [Key, Column(Order = 1)]
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
