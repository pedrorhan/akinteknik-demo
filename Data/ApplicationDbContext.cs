using Microsoft.EntityFrameworkCore;
using AkinTeknik.Web.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<ProductSize> ProductSizes { get; set; } = null!;
    public DbSet<Cart> Carts { get; set; } = null!;
    public DbSet<CartItem> CartItems { get; set; } = null!;
    public DbSet<Slider> Sliders { get; set; } = null!;
    public DbSet<About> Abouts { get; set; } = null!;
    public DbSet<BlogPost> BlogPosts { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Performans ve Hızlı Arama İndeksleri (MySQL index sınırı için MaxLength: 255)
        modelBuilder.Entity<Cart>()
            .Property(c => c.SessionId)
            .HasMaxLength(255);
        modelBuilder.Entity<Cart>()
            .HasIndex(c => c.SessionId);

        modelBuilder.Entity<Product>()
            .Property(p => p.Slug)
            .HasMaxLength(255);
        modelBuilder.Entity<Product>()
            .HasIndex(p => p.Slug);

        modelBuilder.Entity<Category>()
            .Property(c => c.Slug)
            .HasMaxLength(255);
        modelBuilder.Entity<Category>()
            .HasIndex(c => c.Slug);

        modelBuilder.Entity<BlogPost>()
            .Property(b => b.Slug)
            .HasMaxLength(255);
        modelBuilder.Entity<BlogPost>()
            .HasIndex(b => b.Slug);
    }
}