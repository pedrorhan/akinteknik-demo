namespace AkinTeknik.Web.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public int CategoryId { get; set; }
    public string? UsageArea { get; set; }
    public string? Dimensions { get; set; }
    public string? Type { get; set; }
    public string? Detail { get; set; }
    public int Stock { get; set; }
    public bool IsBestSeller { get; set; }
    public Category? Category { get; set; }
    public ICollection<ProductSize> ProductSizes { get; set; } = new List<ProductSize>();
}