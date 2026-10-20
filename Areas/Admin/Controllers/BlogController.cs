using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AkinTeknik.Web.Models;
using AkinTeknik.Web.Helpers;

namespace AkinTeknik.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class BlogController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;

    public BlogController(ApplicationDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        var posts = await _context.BlogPosts.OrderByDescending(b => b.CreatedAt).ToListAsync();
        return View(posts);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BlogPost model, IFormFile? ImageFile)
    {
        if (ModelState.IsValid)
        {
            if (ImageFile != null)
            {
                model.ImageUrl = await ImageHelper.SaveAsWebpAsync(ImageFile, _env.WebRootPath, "blog", maxWidth: 1200);
            }

            model.CreatedAt = DateTime.Now;
            _context.BlogPosts.Add(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Blog yazısı başarıyla oluşturuldu.";
            return RedirectToAction("Index");
        }

        return View(model);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var post = await _context.BlogPosts.FindAsync(id);
        if (post == null) return NotFound();
        return View(post);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(BlogPost model, IFormFile? ImageFile)
    {
        var post = await _context.BlogPosts.FindAsync(model.Id);
        if (post == null) return NotFound();

        if (ImageFile != null)
        {
            post.ImageUrl = await ImageHelper.SaveAsWebpAsync(ImageFile, _env.WebRootPath, "blog", maxWidth: 1200);
        }

        post.Title = model.Title;
        post.Slug = model.Slug;
        post.Summary = model.Summary;
        post.Content = model.Content;
        post.IsPublished = model.IsPublished;

        await _context.SaveChangesAsync();
        TempData["Success"] = "Blog yazısı başarıyla güncellendi";
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var post = await _context.BlogPosts.FindAsync(id);
        if (post != null)
        {

            _context.BlogPosts.Remove(post);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Blog yazısı başarıyla silindi";
        }
        return RedirectToAction("Index");
    }
}