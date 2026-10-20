using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AkinTeknik.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<IActionResult> Index()
    {
        ViewBag.TotalProducts = await _context.Products.CountAsync();
        ViewBag.TotalCategories = await _context.Categories.CountAsync();
        return View();
    }
}