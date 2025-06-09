using Humanizer.Localisation;
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
    [FileTypeValidator(".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tiff")]
    public IFormFile? ProfileImg { get; set; }
    public bool Published { get; set; }
}

public class EditPortfolioDto
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Titel måste anges")]
    [StringLength(64, MinimumLength = 1, ErrorMessage = "Titel måste vara mellan 1 och 64 tecken")]
    [Display(Name = "Titel")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Biografi saknas")]
    [StringLength(1500, ErrorMessage = "Max 1500 tecken")]
    [Display(Name = "Biografi")]
    public string Bio { get; set; } = string.Empty;

    [Display(Name = "Profil Bild")]
    public string? ProfileImage { get; set; }

    [Display(Name = "Profil Bild")]
    [FileTypeValidator(".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tiff")]
    public IFormFile? ProfileImg { get; set; }

}

public class PublicPortfolioDto
{
    public string Name { get; set; } = "";
    public string Title { get; set; } = "";
    public string Bio { get; set; } = "";
    public string? ProfileImage { get; set; }


    public List<ProjectDto> Projects { get; set; } = [];
    public List<SocialLinkDto> SocialLinks { get; set; } = [];
    public List<GitHubRepoDto> GitHubRepos { get; set; } = [];
}


public class FileTypeValidator : ValidationAttribute
{
    private readonly string[] Extensions;
    public FileTypeValidator(params string[] extensions)
        => Extensions = extensions;

    public string GetErrorMessage() =>
        $"Felaktig filtyp.";

    protected override ValidationResult? IsValid(
        object? value, ValidationContext validationContext)
    {
        if (value is IFormFile file)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!Extensions.Contains(ext))
            {
                return new ValidationResult(GetErrorMessage());
            }
        }

                return ValidationResult.Success;
    }
}