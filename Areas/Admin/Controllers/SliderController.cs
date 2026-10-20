using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;
using AkinTeknik.Web.Models;
using AkinTeknik.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AkinTeknik.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class SliderController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;

    public SliderController(ApplicationDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        var sliders = await _context.Sliders.OrderBy(s => s.DisplayOrder).ToListAsync();
        return View(sliders);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Slider slider, IFormFile imageFile)
    {
        if (imageFile != null && imageFile.Length > 0)
        {
            slider.ImageUrl = await ImageHelper.SaveAsWebpAsync(imageFile, _env.WebRootPath, "sliders", maxWidth: 1920);

            _context.Sliders.Add(slider);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        ModelState.AddModelError("", "Lütfen bir görsel seçiniz.");
        return View(slider);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var slider = await _context.Sliders.FindAsync(id);
        if (slider == null)
        {
            return NotFound();
        }
        return View(slider);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Slider slider, IFormFile? imageFile)
    {
        if (id != slider.Id)
        {
            return NotFound();
        }

        var existingSlider = await _context.Sliders.FindAsync(id);
        if (existingSlider == null)
        {
            return NotFound();
        }

        if (imageFile != null && imageFile.Length > 0)
        {
            existingSlider.ImageUrl = await ImageHelper.SaveAsWebpAsync(imageFile, _env.WebRootPath, "sliders", maxWidth: 1920);
        }

        existingSlider.TargetUrl = slider.TargetUrl;
        existingSlider.DisplayOrder = slider.DisplayOrder;

        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var slider = await _context.Sliders.FindAsync(id);
        if (slider != null)
        {
            _context.Sliders.Remove(slider);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Index");
    }
}
