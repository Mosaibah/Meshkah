using System.ComponentModel.DataAnnotations;

namespace Meshkah.ViewModels
{
    public class TransferMoneyVM
    {
        [Display(Name = "المبلغ")]
        public decimal Amount { get; set; }
        public int FromUserId { get; set; }
        [Display(Name = "المحول له")]
        public int ToUserId { get; set; }
    }
}
