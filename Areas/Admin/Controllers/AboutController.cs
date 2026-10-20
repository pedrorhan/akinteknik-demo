using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AkinTeknik.Web.Models;
using AkinTeknik.Web.Helpers;

namespace AkinTeknik.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class AboutController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;

    public AboutController(ApplicationDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var about = await _context.Abouts.FirstOrDefaultAsync();
        return View(about);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(About model, IFormFile? MainImageFile, IFormFile? SecondaryImageFile)
    {
        var about = await _context.Abouts.FirstOrDefaultAsync();
        if (about == null)
        {
            about = new About();
            _context.Abouts.Add(about);
        }

        about.Title = model.Title;
        about.SubTitle = model.SubTitle;
        about.Content = model.Content;
        about.Mission = model.Mission;
        about.Vision = model.Vision;
        about.ExperienceYears = model.ExperienceYears;
        about.ProductCount = model.ProductCount;
        about.CustomerCount = model.CustomerCount;
        about.SatisfactionRate = model.SatisfactionRate;

        string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "about");
        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

        if (MainImageFile != null && MainImageFile.Length > 0)
        {
            about.ImageUrl = await ImageHelper.SaveAsWebpAsync(MainImageFile, _env.WebRootPath, "about", maxWidth: 1200);
        }

        if (SecondaryImageFile != null && SecondaryImageFile.Length > 0)
        {
            about.SecondaryImageUrl = await ImageHelper.SaveAsWebpAsync(SecondaryImageFile, _env.WebRootPath, "about", maxWidth: 800);
        }

        await _context.SaveChangesAsync();
        TempData["Success"] = "Hakkımızda sayfası başarıyla güncellendi!";
        return RedirectToAction("Index");
    }
}