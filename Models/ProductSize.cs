namespace AkinTeknik.Web.Models;

public class ProductSize
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string Size { get; set; } = string.Empty;
    public string? Type { get; set; }
    public string? Detail { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string? ImageUrl { get; set; }
    public Product? Product { get; set; }
}