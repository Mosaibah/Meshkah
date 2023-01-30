using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Meshkah.ViewModels
{
    [Keyless]
    public class AuthVM
    {
        [Display(Name = "الاسم")]
        public string? Name { get; set; }
        [Required]
        [Display(Name = "البريد الالكتروني")]
        public string? Email { get; set; }
        [Required]
        [Display(Name = "كلمة المرور ")]
        public string? Password { get; set; }
    }
}
