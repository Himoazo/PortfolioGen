using System.ComponentModel.DataAnnotations;

namespace PortfolioGen.DTOs;

public class SocialLinkDto
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
}

public class CreateSocialLinkDto
{
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
}

public class UpdateSocialLinkDto
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
}