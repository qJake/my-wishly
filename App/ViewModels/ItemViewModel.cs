using System.ComponentModel.DataAnnotations;

namespace MyWishly.App.ViewModels
{
    public class ItemViewModel
    {
        [Required]
        [Display(Name = "Product Name")]
        public string? Name { get; set; }

        [Required]
        [Display(Name = "Current Price")]
        public double Price { get; set; }

        [Required]
        [Display(Name = "Maximum Price")]
        public double PriceMax { get; set; }

        [Required]
        [Display(Name = "Primary Buy Link")]
        public string? PurchaseUrl { get; set; }

        [Required]
        [Display(Name = "Description / Notes")]
        public string? Description { get; set; }

        [Display(Name = "Primary Product Image")]
        [DataType(DataType.Upload)]
        public IFormFile? Image { get; set; }

        [Display(Name = "Hide from Public Wishlist")]
        public bool IsHidden { get; set; }

        public string? PreviousImageUrl { get; set; }
    }
}
