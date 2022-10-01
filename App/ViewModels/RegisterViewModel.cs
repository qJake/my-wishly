using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyWishly.App.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Give yourself a name.")]
        [MinLength(2)]
        [MaxLength(40)]
        [Display(Name = "Your Name")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Email address must be valid.")]
        [Display(Name = "Email Address")]
        public string? Email { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Give yourself a password of at least 10 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{10,}$", ErrorMessage = "Password must be at least 10 characters, contain one uppercase letter, one lowercase letter, and one number.")]
        [MinLength(10, ErrorMessage = "Password must be at least 10 characters long.")]
        [Display(Name = "Password")]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        [Display(Name = "Password Again")]
        public string? PasswordAgain { get; set; }
    }
}
