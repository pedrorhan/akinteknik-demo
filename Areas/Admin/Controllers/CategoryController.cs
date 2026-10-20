using System.Threading.Tasks;
using AkinTeknik.Web.Models;
using AkinTeknik.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AkinTeknik.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class CategoryController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;

    public CategoryController(ApplicationDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        var categories = await _context.Categories.Include(c => c.ParentCategory).ToListAsync();
        return View(categories);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await _context.Categories.ToListAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category category, IFormFile imageFile)
    {
        if (ModelState.IsValid)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                ModelState.AddModelError("", "Resim eklemek zorunludur!");
                ViewBag.Categories = await _context.Categories.ToListAsync();
                return View(category);
            }
            
            category.ImageUrl = await ImageHelper.SaveAsWebpAsync(imageFile, _env.WebRootPath, "categories", maxWidth: 800);
            string slug = category.Name.ToLower();
            slug = slug.Replace("ş", "s");
            slug = slug.Replace("ı", "i");
            slug = slug.Replace("ğ", "g");
            slug = slug.Replace("ö", "o");
            slug = slug.Replace("ü", "u");
            slug = slug.Replace("ç", "c");
            slug = slug.Replace(" ", "-");
            category.Slug = slug;

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        else
        {
            ViewData["Error"] = "Model is not valid.";
            return View(category);
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        ViewBag.Categories = await _context.Categories.ToListAsync();
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            return NotFound();
        }
        return View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Category category, IFormFile imageFile)
    {
        if (id != category.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                category.ImageUrl = await ImageHelper.SaveAsWebpAsync(imageFile, _env.WebRootPath, "categories", maxWidth: 800);
            }
            string slug = category.Name.ToLower();
            slug = slug.Replace("ş", "s");
            slug = slug.Replace("ı", "i");
            slug = slug.Replace("ğ", "g");
            slug = slug.Replace("ö", "o");
            slug = slug.Replace("ü", "u");
            slug = slug.Replace("ç", "c");
            slug = slug.Replace(" ", "-");
            category.Slug = slug;

            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        else
        {
            ViewData["Error"] = "Model is not valid.";
            return View(category);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        ViewBag.Categories = await _context.Categories.ToListAsync();
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
}