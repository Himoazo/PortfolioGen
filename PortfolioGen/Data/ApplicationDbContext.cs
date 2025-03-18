using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using PortfolioGen.Models;

namespace PortfolioGen.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Portfolio> Portfolios { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<SocialLink> SocialLinks { get; set; }
}
