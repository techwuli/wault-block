using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WaultBlock.Web.Models
{
    [Table("IndyWallets")]
    public class IndyWallet
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public string Name { get; set; }
    }
}
