namespace PortfolioGen.Models;

public class SocialLink
{
    public int Id { get; set; }
    
    public string Platform { get; set; }
    public string Url { get; set; }

    public int PortfolioId { get; set; }
    public Portfolio Portfolio { get; set; }
}
