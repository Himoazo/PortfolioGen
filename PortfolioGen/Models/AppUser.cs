using Microsoft.AspNetCore.Identity;

namespace PortfolioGen.Models;

public class AppUser : IdentityUser
{
    public Portfolio? Portfolio { get; set; }
}
