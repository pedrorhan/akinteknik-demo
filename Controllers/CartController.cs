using System.Threading.Tasks;
using AkinTeknik.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AkinTeknik.Web.Controllers;

public class CartController : Controller
{
    private const int MaxQuantity = 9999;
    private readonly ApplicationDbContext _context;

    public CartController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        string? sessionId = Request.Cookies["AkinSession"];

        if (!string.IsNullOrEmpty(sessionId))
        {
            var cart = await _context.Carts
                .Include(c => c.Items).ThenInclude(i => i.Product)
                .Include(c => c.Items).ThenInclude(i => i.Variant)
                .FirstOrDefaultAsync(c => c.SessionId == sessionId);

            return View(cart);
        }
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int ProductId, int? VariantId, int Quantity = 1)
    {
        // 1. Miktar Doğrulaması (M3)
        if (Quantity < 1 || Quantity > MaxQuantity)
        {
            Quantity = 1;
        }

        // 2. Ürün ve Varyant Doğrulaması (M4, M5)
        var product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == ProductId);
        if (product == null)
        {
            return NotFound();
        }

        if (VariantId.HasValue && VariantId.Value > 0)
        {
            var variantBelongsToProduct = await _context.ProductSizes
                .AsNoTracking()
                .AnyAsync(v => v.Id == VariantId.Value && v.ProductId == ProductId);

            if (!variantBelongsToProduct)
            {
                return BadRequest("Geçersiz ürün varyantı seçimi.");
            }
        }
        else
        {
            VariantId = null;
        }

        // 3. Güvenli Session & Çerez Yönetimi (O5)
        string? sessionId = Request.Cookies["AkinSession"];
        if (string.IsNullOrEmpty(sessionId) || !Guid.TryParse(sessionId, out _))
        {
            sessionId = Guid.NewGuid().ToString();
            var cookieOptions = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(30),
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Lax,
                IsEssential = true
            };
            Response.Cookies.Append("AkinSession", sessionId, cookieOptions);
        }

        // 4. Sepet Getirme veya Oluşturma (CreatedAt tanımlandı)
        var cart = await _context.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.SessionId == sessionId);
        if (cart == null)
        {
            cart = new Cart
            {
                SessionId = sessionId,
                CreatedAt = DateTime.UtcNow
            };
            _context.Carts.Add(cart);
        }

        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == ProductId && i.VariantId == VariantId);
        if (existingItem != null)
        {
            existingItem.Quantity = Math.Min(existingItem.Quantity + Quantity, MaxQuantity);
        }
        else
        {
            var newItem = new CartItem
            {
                ProductId = ProductId,
                VariantId = VariantId,
                Quantity = Quantity
            };
            cart.Items.Add(newItem);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Increase(int id)
    {
        var item = await GetOwnedCartItemAsync(id);
        if (item != null)
        {
            if (item.Quantity < MaxQuantity)
            {
                item.Quantity++;
                await _context.SaveChangesAsync();
            }
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Decrease(int id)
    {
        var item = await GetOwnedCartItemAsync(id);
        if (item != null)
        {
            if (item.Quantity > 1)
            {
                item.Quantity--;
            }
            else
            {
                _context.CartItems.Remove(item);
            }
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int id)
    {
        var item = await GetOwnedCartItemAsync(id);
        if (item != null)
        {
            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Index");
    }

    // IDOR Koruması: Yalnızca mevcut kullanıcının sepetine ait kalemi getirir (O2)
    private async Task<CartItem?> GetOwnedCartItemAsync(int id)
    {
        string? sessionId = Request.Cookies["AkinSession"];
        if (string.IsNullOrEmpty(sessionId)) return null;

        return await _context.CartItems
            .Include(ci => ci.Cart)
            .FirstOrDefaultAsync(ci => ci.Id == id && ci.Cart.SessionId == sessionId);
    }
}