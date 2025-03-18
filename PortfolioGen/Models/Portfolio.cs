using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PortfolioGen.Models;

public class Portfolio
{
    public int Id { get; set; }
    public string UsernameSlug { get; set; }  = Guid.NewGuid().ToString("N")[..8];

    [Required(ErrorMessage = "Titel måste anges")]
    [StringLength(64, MinimumLength = 1, ErrorMessage = "Titel måste vara mellan 1 och 64 tecken")]
    [Display(Name = "Titel")]
    public string Title { get; set; } = string.Empty;
    [Required(ErrorMessage = "Biografi saknas")]
    [StringLength(1500, ErrorMessage = "Max 1500 tecken")]
    [Display(Name = "Biografi")]
    public string Bio { get; set; } = string.Empty;

    [Display(Name = "Bild")]
    public string? ProfileImageUrl { get; set; }
   
    public bool Published { get; set; }
    public DateOnly CreatedAt { get; set; }

    public required string AppUserId { get; set; }
    public required AppUser AppUser { get; set; }
    public List<Project> Projects { get; set; } = [];
    public List<SocialLink> SocialLinks { get; set; } = [];
}

