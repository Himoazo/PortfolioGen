using Microsoft.AspNetCore.Identity;

namespace PortfolioGen.Models;

public class AppUser : IdentityUser
{
    public int? PortfolioId  { get; set; }
    public Portfolio? Portfolio { get; set; }
}
