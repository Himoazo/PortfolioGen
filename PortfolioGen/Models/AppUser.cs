using Microsoft.AspNetCore.Identity;
using PortfolioGen.DTOs;
using System.ComponentModel.DataAnnotations;

namespace PortfolioGen.Models;

public class AppUser : IdentityUser
{
    [Required(ErrorMessage = "Du måste ange ditt namn")]
    [StringLength(255, ErrorMessage = "Namn kan inte vara längre än 255 tecken")]
    [Display(Name = "För och efternamn")]
    public string Name { get; set; } = string.Empty;

    

    public Portfolio? Portfolio { get; set; }
}
