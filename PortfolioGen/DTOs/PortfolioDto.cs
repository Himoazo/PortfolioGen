using System.ComponentModel.DataAnnotations;

namespace PortfolioGen.DTOs;

public class PortfolioDto
{
    [Required(ErrorMessage = "Titel måste anges")]
    [StringLength(64, MinimumLength = 1, ErrorMessage = "Titel måste vara mellan 1 och 64 tecken")]
    [Display(Name = "Titel")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Biografi saknas")]
    [StringLength(1500, ErrorMessage = "Max 1500 tecken")]
    [Display(Name = "Biografi")]
    public string Bio { get; set; } = string.Empty;

    [Display(Name = "Profil Bild")]
    public IFormFile? ProfileImg { get; set; }

    public bool Published { get; set; }
}

