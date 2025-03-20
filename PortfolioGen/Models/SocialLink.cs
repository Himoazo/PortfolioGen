using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PortfolioGen.Models;

public class SocialLink
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Plattform måste anges")]
    [StringLength(50, ErrorMessage = "Max 50 tecken")]
    [Display(Name = "Plattform")]
    public string Platform { get; set; } = string.Empty;

    [Required(ErrorMessage = "URL måste anges")]
    [Url(ErrorMessage = "Ogiltig URL")]
    [StringLength(255, ErrorMessage = "Max 255 tecken")]
    [Display(Name = "URL")]
    public string Url { get; set; } = string.Empty;

    public int PortfolioId { get; set; }
    [JsonIgnore]
    public Portfolio Portfolio { get; set; } = null!;
}