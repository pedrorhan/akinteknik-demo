using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AkinTeknik.Web.Models;

namespace AkinTeknik.Web.Controllers;

public class BlogController : Controller
{
    private readonly ApplicationDbContext _context;

    public BlogController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var posts = await _context.BlogPosts
            .AsNoTracking()
            .Where(b => b.IsPublished)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        return View(posts);
    }

    [Route("Blog/{slug}")]
    public async Task<IActionResult> Detail(string slug)
    {
        var post = await _context.BlogPosts
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Slug == slug && b.IsPublished);

        if (post == null)
        {
            return NotFound();
        }

        return View(post);
    }
}