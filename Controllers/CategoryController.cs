using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AkinTeknik.Web.Helpers;

namespace AkinTeknik.Web.Controllers;

public class CategoryController : Controller
{
    private readonly ApplicationDbContext _context;
    public CategoryController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var categories = await _context.Categories.AsNoTracking().ToListAsync();
        return View(categories);
    }

    [Route("Kategori/{slug}")]
    public async Task<IActionResult> Detail(string slug)
    {
        var allCategories = await _context.Categories.AsNoTracking().ToListAsync();
        CategoryHelper.BuildCategoryHierarchy(allCategories);
        var categoryMap = allCategories.ToDictionary(c => c.Id);

        var category = allCategories.FirstOrDefault(c => c.Slug == slug);
        if (category == null)
        {
            return NotFound();
        }

        ViewBag.Categories = allCategories;
        ViewBag.CategoryMap = categoryMap;
        ViewBag.ActivePathIds = CategoryHelper.GetActivePathIds(category.Id, categoryMap);
        ViewBag.BreadcrumbTrail = CategoryHelper.GetBreadcrumbTrail(category.Id, categoryMap);

        var descendantIds = CategoryHelper.GetAllDescendantIds(category.Id, allCategories);

        category.Products = await _context.Products
            .AsNoTracking()
            .Where(p => descendantIds.Contains(p.CategoryId))
            .Include(p => p.Category)
            .OrderBy(p => p.Id) // Varsayılan: Eskiden yeniye
            .ToListAsync();

        return View("Detail", category);
    }
}