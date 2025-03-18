using Microsoft.AspNetCore.Identity;

namespace PortfolioGen.Models;

public class Portfolio
{
    public int Id { get; set; }
    public string UsernameSlug { get; set; }  // Unique 
    public string Title { get; set; }
    public string Bio { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string Theme { get; set; }
    public bool Published { get; set; }
    public DateOnly CreatedAt { get; set; }

    public string UserId { get; set; }
    public AppUser AppUser { get; set; }
    public List<Project> Projects { get; set; }
    public List<SocialLink> SocialLinks { get; set; }
}

