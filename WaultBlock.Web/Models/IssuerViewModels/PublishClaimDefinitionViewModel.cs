using System.ComponentModel.DataAnnotations;

namespace WaultBlock.Web.Models.IssuerViewModels
{
    public class PublishClaimDefinitionViewModel
    {
        [Required]
        public string WalletName { get; set; }
    }
}
