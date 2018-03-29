using System.ComponentModel.DataAnnotations;

namespace WaultBlock.Web.Models.IdentitiesViewModels
{
    public class CreateWalletViewModel
    {
        [Required]
        public string Name { get; set; }
    }
}
