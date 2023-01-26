using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Meshkah.ViewModels
{
    [Keyless]
    public class AuthVM
    {
        public string? Name { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
