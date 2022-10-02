using System.ComponentModel.DataAnnotations;

namespace MyWishly.App.ViewModels
{
    public class SettingsViewModel
    {
        [Display(Name = "Friendly URL")]
        [RegularExpression(@"^[a-zA-Z0-9_-]{3,25}$", ErrorMessage = "Friendly URL can only consist of letters and numbers, _, -, and must be between 3 and 25 characters long.")]
        public string? FriendlyUrl { get; set; }
    }
}
