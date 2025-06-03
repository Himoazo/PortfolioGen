using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PortfolioGen.Models;

public class Project
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Titel måste anges")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Titel måste vara mellan 1 och 100 tecken")]
    [Display(Name = "Titel")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Beskrivning saknas")]
    [StringLength(250, ErrorMessage = "Max 250 tecken")]
    [Display(Name = "Beskrivning")]
    public string Description { get; set; } = string.Empty;

    [Url(ErrorMessage = "Ogiltig URL")]
    [Display(Name = "Projekt URL")]
    public string? ProjectUrl { get; set; }

    [Url(ErrorMessage = "Ogiltig URL")]
    [Display(Name = "Github-repo URL")]
    public string? GithubUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public int PortfolioId { get; set; }
    [JsonIgnore]
    public Portfolio Portfolio { get; set; } = null!;
}