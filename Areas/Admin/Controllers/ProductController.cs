using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AkinTeknik.Web.Models;
using AkinTeknik.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AkinTeknik.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class ProductController : Controller
{
    private readonly ApplicationDbContext _context;
    private IWebHostEnvironment _env;

    public ProductController(ApplicationDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.ProductSizes)
            .OrderBy(p => p.Id) // Varsayılan: Eskiden yeniye
            .ToListAsync();
        return View(products);
    }

    private async Task PopulateCategoriesViewBag()
    {
        var allCategories = await _context.Categories.AsNoTracking().ToListAsync();
        CategoryHelper.BuildCategoryHierarchy(allCategories);
        var categoryMap = allCategories.ToDictionary(c => c.Id);
        ViewBag.CategoryMap = categoryMap;
        ViewBag.Categories = allCategories
            .OrderBy(c => c.ParentCategoryId == null ? 0 : 1)
            .ThenBy(c => CategoryHelper.GetFullHierarchyName(c.Id, categoryMap))
            .ToList();
    }

    public async Task<IActionResult> Create()
    {
        await PopulateCategoriesViewBag();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product, IFormFile? imageFile)
    {
        ModelState.Remove("Category");
        ModelState.Remove("ProductSizes");
        ModelState.Remove("imageFile");

        if (ModelState.IsValid)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                ModelState.AddModelError("", "Resim eklemek zorunludur!");
                await PopulateCategoriesViewBag();
                return View(product);
            }
            
            product.ImageUrl = await ImageHelper.SaveAsWebpAsync(imageFile, _env.WebRootPath, "products", maxWidth: 1000);
            product.Slug = GenerateSlug(product.Name);
            
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Ürün başarıyla eklendi.";
            return RedirectToAction("Index");
        }

        await PopulateCategoriesViewBag();
        return View(product);
    }

    public async Task<IActionResult> Edit(int id)
    {
        await PopulateCategoriesViewBag();

        var product = await _context.Products.Include(p => p.ProductSizes).FirstOrDefaultAsync(p => p.Id == id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Product product, IFormFile? imageFile)
    {
        if (id != product.Id)
        {
            return NotFound();
        }

        ModelState.Remove("Category");
        ModelState.Remove("ProductSizes");
        ModelState.Remove("imageFile");

        if (ModelState.IsValid)
        {
            var existingProduct = await _context.Products.Include(p => p.ProductSizes).FirstOrDefaultAsync(p => p.Id == id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            existingProduct.Name = product.Name;
            existingProduct.CategoryId = product.CategoryId;
            existingProduct.Price = product.Price;
            existingProduct.Stock = product.Stock;
            existingProduct.UsageArea = product.UsageArea;
            existingProduct.Dimensions = product.Dimensions;
            existingProduct.Type = product.Type;
            existingProduct.Detail = product.Detail;
            existingProduct.Description = product.Description;
            existingProduct.IsBestSeller = product.IsBestSeller;

            if (imageFile != null && imageFile.Length > 0)
            {
                existingProduct.ImageUrl = await ImageHelper.SaveAsWebpAsync(imageFile, _env.WebRootPath, "products", maxWidth: 1000);
            }

            existingProduct.Slug = GenerateSlug(product.Name);

            await _context.SaveChangesAsync();
            TempData["Success"] = "Ürün başarıyla güncellendi.";
            return RedirectToAction("Index");
        }

        await PopulateCategoriesViewBag();
        return View(product);
    }

    private static string GenerateSlug(string text)
    {
        if (string.IsNullOrEmpty(text))
            return "";

        text = text.ToLowerInvariant();
        text = text.Replace("ç", "c").Replace("ğ", "g").Replace("ı", "i").Replace("ö", "o").Replace("ş", "s").Replace("ü", "u");
        text = System.Text.RegularExpressions.Regex.Replace(text, @"[^a-z0-9\s-]", "");
        text = System.Text.RegularExpressions.Regex.Replace(text, @"\s+", " ").Trim();
        text = text.Replace(" ", "-");
        return text;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Ürün başarıyla silindi.";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> AddSize(int productId, string size, decimal price)
    {
        var product = await _context.Products.FindAsync(productId);
        if(product == null) return NotFound();

        var pSize = new ProductSize
        {
            ProductId = productId,
            Size = size,
            Price = price
        };

        _context.ProductSizes.Add(pSize);
        await _context.SaveChangesAsync();
        return RedirectToAction("Edit", new { id = productId });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteSize(int id)
    {
        var pSize = await _context.ProductSizes.FindAsync(id);
        if(pSize == null) return NotFound();

        int pId = pSize.ProductId;
        _context.ProductSizes.Remove(pSize);
        await _context.SaveChangesAsync();
        return RedirectToAction("Edit", new { id = pId });
    }
}