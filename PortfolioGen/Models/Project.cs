namespace PortfolioGen.Models;

public class Project
{
    public int Id { get; set; }
    
    public string Title { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public string ProjectUrl { get; set; }
    public DateTime CreatedAt { get; set; }

    public int PortfolioId { get; set; }
    public Portfolio Portfolio { get; set; }
}
