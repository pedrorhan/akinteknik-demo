using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AkinTeknik.Web.Helpers;

namespace AkinTeknik.Web.Controllers;

public class ProductController : Controller
{
    private readonly ApplicationDbContext _context;
    public ProductController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var allCategories = await _context.Categories.AsNoTracking().ToListAsync();
        CategoryHelper.BuildCategoryHierarchy(allCategories);
        ViewBag.Categories = allCategories;
        ViewBag.CategoryMap = allCategories.ToDictionary(c => c.Id);

        var products = await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .OrderBy(p => p.Id) // Varsayılan: Eskiden yeniye
            .ToListAsync();

        return View(products);
    }
}