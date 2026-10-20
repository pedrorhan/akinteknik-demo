namespace AkinTeknik.Web.Models;

public class About
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? SubTitle { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? SecondaryImageUrl { get; set; }
    public string? Mission { get; set; }
    public string? Vision { get; set; }
    public string? ExperienceYears { get; set; }
    public string? ProductCount { get; set; }
    public string? CustomerCount { get; set; }
    public string? SatisfactionRate { get; set; }
}