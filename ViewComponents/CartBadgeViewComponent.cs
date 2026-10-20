using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AkinTeknik.Web.Models;

namespace AkinTeknik.Web.ViewComponents;

public class CartBadgeViewComponent : ViewComponent
{
    private readonly ApplicationDbContext _context;

    public CartBadgeViewComponent(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        int cartCount = 0;
        string? sessionId = Request.Cookies["AkinSession"];
        if (!string.IsNullOrEmpty(sessionId) && Guid.TryParse(sessionId, out _))
        {
            cartCount = await _context.Carts
                .AsNoTracking()
                .Where(c => c.SessionId == sessionId)
                .SelectMany(c => c.Items)
                .CountAsync();
        }

        return View(cartCount);
    }
}
