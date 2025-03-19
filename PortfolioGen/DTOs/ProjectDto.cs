using System.ComponentModel.DataAnnotations;

namespace PortfolioGen.DTOs;

public class ProjectDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Titel måste anges")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Titel måste vara mellan 1 och 100 tecken")]
    [Display(Name = "Titel")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Beskrivning saknas")]
    [StringLength(1000, ErrorMessage = "Max 1000 tecken")]
    [Display(Name = "Beskrivning")]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "Bild URL")]
    public string? ImageUrl { get; set; }

    [Url(ErrorMessage = "Ogiltig URL")]
    [Display(Name = "Projekt URL")]
    public string? ProjectUrl { get; set; }

    public int PortfolioId { get; set; }
}


public class CreateProjectDto
{
    [Required(ErrorMessage = "Titel måste anges")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Titel måste vara mellan 1 och 100 tecken")]
    [Display(Name = "Titel")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Beskrivning saknas")]
    [StringLength(1000, ErrorMessage = "Max 1000 tecken")]
    [Display(Name = "Beskrivning")]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "Bild URL")]
    public string? ImageUrl { get; set; }

    [Url(ErrorMessage = "Ogiltig URL")]
    [Display(Name = "Projekt URL")]
    public string? ProjectUrl { get; set; }

    public int PortfolioId { get; set; }
}


public class UpdateProjectDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Titel måste anges")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Titel måste vara mellan 1 och 100 tecken")]
    [Display(Name = "Titel")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Beskrivning saknas")]
    [StringLength(1000, ErrorMessage = "Max 1000 tecken")]
    [Display(Name = "Beskrivning")]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "Bild URL")]
    public string? ImageUrl { get; set; }

    [Url(ErrorMessage = "Ogiltig URL")]
    [Display(Name = "Projekt URL")]
    public string? ProjectUrl { get; set; }
}