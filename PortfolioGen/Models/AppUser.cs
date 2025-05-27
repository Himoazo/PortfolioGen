using Microsoft.AspNetCore.Identity;
using PortfolioGen.DTOs;
using System.ComponentModel.DataAnnotations;

namespace PortfolioGen.Models;

public class AppUser : IdentityUser
{
    [Required(ErrorMessage = "Förnamn måste anges")]
    [StringLength(64, ErrorMessage = "Namn kan inte vara längre än 64 tecken")]
    [Display(Name = "Förnamn")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Efternamn måste anges")]
    [StringLength(64, ErrorMessage = "Efternamn kan inte vara längre än 64 tecken")]
    [Display(Name = "Efternamn")]
    public string? LastName { get; set; } = string.Empty;

    public Portfolio? Portfolio { get; set; }
}
