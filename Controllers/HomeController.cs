using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AkinTeknik.Web.Models;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AkinTeknik.Web.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<HomeController> _logger;
    private readonly IMemoryCache _cache;

    public HomeController(ApplicationDbContext context, IConfiguration configuration, ILogger<HomeController> logger, IMemoryCache cache)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
        _cache = cache;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.Sliders = await _context.Sliders.AsNoTracking().OrderBy(s => s.DisplayOrder).ToListAsync();
        ViewBag.Categories = await _context.Categories
            .AsNoTracking()
            .Where(c => c.ParentCategoryId == null)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToListAsync();

        var bestSellers = await _context.Products
            .AsNoTracking()
            .Where(p => p.IsBestSeller)
            .Include(p => p.Category)
            .OrderBy(p => p.Id)
            .Take(4)
            .ToListAsync();
        return View(bestSellers);
    }

    [Route("Urun/{slug}")]
    public async Task<IActionResult> ProductDetail(string slug)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.ProductSizes)
            .FirstOrDefaultAsync(p => p.Slug == slug);

        if (product == null)
        {
            return NotFound();
        }

        ViewBag.RelatedProducts = await _context.Products
            .AsNoTracking()
            .Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id)
            .Take(4)
            .ToListAsync();

        return View(product);
    }

    public async Task<IActionResult> About()
    {
        var about = await _context.Abouts.FirstOrDefaultAsync();
        return View(about);
    }

    public IActionResult Contact()
    {
        return View();
    }

    [HttpGet]
    [Route("gizlilik-politikasi")]
    [Route("Home/Privacy")]
    public IActionResult Privacy()
    {
        return View();
    }

    [HttpGet]
    [Route("kullanim-sartlari")]
    [Route("Home/Terms")]
    public IActionResult Terms()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendMessage(string FullName, string Phone, string Email, string? Subject, string Message, string? Website)
    {
        // 1. Bot Koruması (Honeypot): Gizli alanı dolduran botları sessizce savuştur
        if (!string.IsNullOrWhiteSpace(Website))
        {
            _logger.LogWarning("Honeypot bot yakalandı. IP: {IP}", HttpContext.Connection.RemoteIpAddress);
            TempData["Success"] = "Mesajınız başarıyla iletildi.";
            return RedirectToAction("Contact");
        }

        // 2. Rate Limiting (Aşırı istek / spam flood engelleme - IP başına 60 sn bekleme)
        var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown_client";
        var rateLimitKey = $"Contact_Limit_{clientIp}";
        if (_cache.TryGetValue(rateLimitKey, out _))
        {
            TempData["Error"] = "Çok sık mesaj gönderdiniz. Lütfen 1 dakika sonra tekrar deneyin.";
            return RedirectToAction("Contact");
        }

        if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Message) ||
            string.IsNullOrWhiteSpace(Email) || Message.Length > 5000 ||
            !MailAddress.TryCreate(Email, out var replyToAddress))
        {
            TempData["Error"] = "Lütfen form alanlarını eksiksiz ve geçerli bir e-posta adresiyle doldurun.";
            return RedirectToAction("Contact");
        }

        // Başarılı gönderim öncesi rate limit kilidini 60 saniyeliğine koy
        _cache.Set(rateLimitKey, true, TimeSpan.FromMinutes(1));

        try
        {
            Func<string?, string> enc = s => WebUtility.HtmlEncode(s ?? string.Empty);
            var safeSubject = (Subject ?? FullName).Replace("\r", " ").Replace("\n", " ");

            string body = $@"
            <h3>Yeni İletişim / Teklif Formu Mesajı</h3>
            <p><strong>Ad Soyad:</strong> {enc(FullName)}</p>
            <p><strong>Telefon:</strong> {enc(Phone ?? "Belirtilmedi")}</p>
            <p><strong>E-Posta:</strong> {enc(Email)}</p>
            <p><strong>Konu:</strong> {enc(safeSubject)}</p>
            <hr />
            <p><strong>Mesaj:</strong><br />{enc(Message).Replace("\n", "<br />")}</p>
            ";

            string smtpServer = _configuration["EmailSettings:SmtpServer"] ?? "smtp.gmail.com";
            int port = int.TryParse(_configuration["EmailSettings:Port"], out var p) ? p : 587;
            string? senderEmail = _configuration["EmailSettings:SenderEmail"];
            string? appPassword = _configuration["EmailSettings:AppPassword"]?.Replace(" ", "");
            string receiverEmail = _configuration["EmailSettings:ReceiverEmail"] ?? senderEmail ?? "";

            if (string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(appPassword))
            {
                _logger.LogWarning("EmailSettings yapılandırılmadığı için e-posta gönderilemedi.");
                TempData["Error"] = "E-posta servisi şu anda aktif değil. Lütfen WhatsApp veya telefon ile iletişime geçin.";
                return RedirectToAction("Contact");
            }

            using (var client = new SmtpClient(smtpServer, port))
            {
                client.EnableSsl = true;
                client.Timeout = 10000;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(senderEmail, appPassword);

                using (var mail = new MailMessage())
                {
                    mail.From = new MailAddress(senderEmail, "Akın Teknik Web Formu");
                    mail.To.Add(receiverEmail);
                    mail.Subject = $"Web Sitesi Mesajı: {safeSubject}";
                    mail.Body = body;
                    mail.IsBodyHtml = true;
                    mail.ReplyToList.Add(replyToAddress);
                    await client.SendMailAsync(mail);
                }
            }
            TempData["Success"] = $"Sayın {FullName}, mesajınız başarıyla bize iletildi. En kısa sürede sizinle iletişime geçeceğiz.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "İletişim formu mesajı gönderilirken hata oluştu: {Email}", Email);
            TempData["Error"] = "Mesajınız iletilirken teknik bir hata oluştu, lütfen telefon veya WhatsApp hattımızdan bize ulaşın.";
        }
        return RedirectToAction("Contact");
    }

    [HttpGet]
    [Route("sitemap.xml")]
    public async Task<IActionResult> Sitemap()
    {
        var configBaseUrl = _configuration["SiteSettings:BaseUrl"]?.TrimEnd('/');
        var baseUrl = !string.IsNullOrEmpty(configBaseUrl)
            ? configBaseUrl
            : $"{Request.Scheme}://{Request.Host}";

        var xmlContent = await _cache.GetOrCreateAsync("SitemapXmlContent", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
            var today = DateTime.UtcNow.ToString("yyyy-MM-dd");

            var xml = new StringBuilder();
            xml.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            xml.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

            // 1. Statik Sayfalar
            var staticPages = new List<(string Path, string ChangeFreq, string Priority)>
            {
                ("/", "daily", "1.0"),
                ("/Product", "daily", "0.9"),
                ("/Category", "weekly", "0.8"),
                ("/Blog", "daily", "0.8"),
                ("/Home/About", "monthly", "0.7"),
                ("/Home/Contact", "monthly", "0.7"),
                ("/gizlilik-politikasi", "yearly", "0.3"),
                ("/kullanim-sartlari", "yearly", "0.3")
            };

            foreach (var page in staticPages)
            {
                xml.AppendLine("  <url>");
                xml.AppendLine($"    <loc>{baseUrl}{page.Path}</loc>");
                xml.AppendLine($"    <lastmod>{today}</lastmod>");
                xml.AppendLine($"    <changefreq>{page.ChangeFreq}</changefreq>");
                xml.AppendLine($"    <priority>{page.Priority}</priority>");
                xml.AppendLine("  </url>");
            }

            // 2. Dinamik Kategoriler
            var categorySlugs = await _context.Categories
                .AsNoTracking()
                .Where(c => !string.IsNullOrEmpty(c.Slug))
                .Select(c => c.Slug)
                .ToListAsync();

            foreach (var slug in categorySlugs)
            {
                xml.AppendLine("  <url>");
                xml.AppendLine($"    <loc>{baseUrl}/Kategori/{slug}</loc>");
                xml.AppendLine($"    <lastmod>{today}</lastmod>");
                xml.AppendLine("    <changefreq>weekly</changefreq>");
                xml.AppendLine("    <priority>0.8</priority>");
                xml.AppendLine("  </url>");
            }

            // 3. Dinamik Ürünler
            var productSlugs = await _context.Products
                .AsNoTracking()
                .Where(p => !string.IsNullOrEmpty(p.Slug))
                .Select(p => p.Slug)
                .ToListAsync();

            foreach (var slug in productSlugs)
            {
                xml.AppendLine("  <url>");
                xml.AppendLine($"    <loc>{baseUrl}/Urun/{slug}</loc>");
                xml.AppendLine($"    <lastmod>{today}</lastmod>");
                xml.AppendLine("    <changefreq>weekly</changefreq>");
                xml.AppendLine("    <priority>0.8</priority>");
                xml.AppendLine("  </url>");
            }

            // 4. Dinamik Blog Yazıları
            var blogPosts = await _context.BlogPosts
                .Where(b => b.IsPublished && !string.IsNullOrEmpty(b.Slug))
                .AsNoTracking()
                .Select(b => new { b.Slug, b.CreatedAt })
                .ToListAsync();

            foreach (var post in blogPosts)
            {
                var postDate = post.CreatedAt.ToString("yyyy-MM-dd");
                xml.AppendLine("  <url>");
                xml.AppendLine($"    <loc>{baseUrl}/Blog/{post.Slug}</loc>");
                xml.AppendLine($"    <lastmod>{postDate}</lastmod>");
                xml.AppendLine("    <changefreq>monthly</changefreq>");
                xml.AppendLine("    <priority>0.7</priority>");
                xml.AppendLine("  </url>");
            }

            xml.AppendLine("</urlset>");
            return xml.ToString();
        });

        return Content(xmlContent ?? string.Empty, "application/xml", Encoding.UTF8);
    }

    [HttpGet]
    [Route("robots.txt")]
    public IActionResult Robots()
    {
        var configBaseUrl = _configuration["SiteSettings:BaseUrl"]?.TrimEnd('/');
        var baseUrl = !string.IsNullOrEmpty(configBaseUrl)
            ? configBaseUrl
            : $"{Request.Scheme}://{Request.Host}";
        var content = new StringBuilder();
        content.AppendLine("User-agent: *");
        content.AppendLine("Allow: /");
        content.AppendLine("Disallow: /Admin/");
        content.AppendLine("Disallow: /Cart/");
        content.AppendLine();
        content.AppendLine($"Sitemap: {baseUrl}/sitemap.xml");
        return Content(content.ToString(), "text/plain", Encoding.UTF8);
    }

    [HttpGet]
    [Route("404")]
    [Route("Home/NotFoundPage")]
    public async Task<IActionResult> NotFoundPage()
    {
        Response.StatusCode = 404;
        ViewBag.Categories = await _context.Categories.Take(6).ToListAsync();
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpGet]
    [Route("llms.txt")]
    public async Task<IActionResult> Llms()
    {
        var configBaseUrl = _configuration["SiteSettings:BaseUrl"]?.TrimEnd('/');
        var baseUrl = !string.IsNullOrEmpty(configBaseUrl)
            ? configBaseUrl
            : $"{Request.Scheme}://{Request.Host}";

        var content = await _cache.GetOrCreateAsync("LlmsTxtContent", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);

            var sb = new StringBuilder();
            sb.AppendLine("# Akın Teknik & Kuzey Parlatma Ekipmanları");
            sb.AppendLine();
            sb.AppendLine("> Akın Teknik, Türkiye merkezli endüstriyel kalıp sarf malzemeleri, kalıp elemanları ve parlatma (polisaj) ekipmanları toptancısı ve üreticisidir. Tescilli alt markası Kuzey ile sanayi kuruluşlarına, kalıp atölyelerine ve imalatçılara toptan malzeme ve teknik hırdavat tedariki sağlamaktadır.");
            sb.AppendLine();
            sb.AppendLine("## Kurumsal Bilgiler & İletişim");
            sb.AppendLine();
            sb.AppendLine($"- Web Sitesi: {baseUrl}");
            sb.AppendLine("- Telefon / WhatsApp: +90 555 000 00 00");
            sb.AppendLine("- E-Posta: info@demoteknik.com");
            sb.AppendLine("- Markalar: Akın Teknik Demo, Kuzey Parlatma Ekipmanları");
            sb.AppendLine("- Hizmet Modeli: Toptan satış, teklif listesi oluşturma ve teknik kalıp sarf malzeme tedariki");
            sb.AppendLine();
            sb.AppendLine("## Ürün Kategorileri");
            sb.AppendLine();

            var categories = await _context.Categories
                .AsNoTracking()
                .Where(c => !string.IsNullOrEmpty(c.Slug))
                .Select(c => new { c.Name, c.Slug, c.Description })
                .ToListAsync();

            foreach (var cat in categories)
            {
                var desc = !string.IsNullOrEmpty(cat.Description) 
                    ? cat.Description.Replace("\r", " ").Replace("\n", " ").Trim() 
                    : $"{cat.Name} kalıp ve parlatma sarf malzemeleri.";
                sb.AppendLine($"- [{cat.Name}]({baseUrl}/Kategori/{cat.Slug}): {desc}");
            }

            sb.AppendLine();
            sb.AppendLine("## Öne Çıkan Ürünler");
            sb.AppendLine();

            var products = await _context.Products
                .AsNoTracking()
                .Where(p => !string.IsNullOrEmpty(p.Slug))
                .OrderByDescending(p => p.Id)
                .Take(20)
                .Select(p => new { p.Name, p.Slug, CategoryName = p.Category != null ? p.Category.Name : "Kalıp Malzemeleri", p.Description })
                .ToListAsync();

            foreach (var prod in products)
            {
                var catName = !string.IsNullOrEmpty(prod.CategoryName) ? prod.CategoryName : "Kalıp Malzemeleri";
                var prodDesc = !string.IsNullOrEmpty(prod.Description) 
                    ? prod.Description.Replace("\r", " ").Replace("\n", " ").Trim() 
                    : $"{prod.Name} endüstriyel kalıp ve parlatma ekipmanı.";
                if (prodDesc.Length > 100) prodDesc = prodDesc.Substring(0, 97) + "...";
                sb.AppendLine($"- [{prod.Name}]({baseUrl}/Urun/{prod.Slug}): {catName} kategorisinde. {prodDesc}");
            }

            sb.AppendLine();
            sb.AppendLine("## Blog & Teknik Makaleler");
            sb.AppendLine();

            var posts = await _context.BlogPosts
                .Where(b => b.IsPublished && !string.IsNullOrEmpty(b.Slug))
                .AsNoTracking()
                .Select(b => new { b.Title, b.Slug, b.Summary })
                .ToListAsync();

            foreach (var post in posts)
            {
                var postDesc = !string.IsNullOrEmpty(post.Summary) 
                    ? post.Summary.Replace("\r", " ").Replace("\n", " ").Trim() 
                    : post.Title;
                sb.AppendLine($"- [{post.Title}]({baseUrl}/Blog/{post.Slug}): {postDesc}");
            }

            sb.AppendLine();
            sb.AppendLine("## Hızlı Bağlantılar");
            sb.AppendLine();
            sb.AppendLine($"- [Ana Sayfa]({baseUrl}/)");
            sb.AppendLine($"- [Ürün Kataloğu]({baseUrl}/Product)");
            sb.AppendLine($"- [Kategoriler]({baseUrl}/Category)");
            sb.AppendLine($"- [Blog]({baseUrl}/Blog)");
            sb.AppendLine($"- [Hakkımızda & Kuzey]({baseUrl}/Home/About)");
            sb.AppendLine($"- [İletişim & Teklif Formu]({baseUrl}/Home/Contact)");
            sb.AppendLine($"- [Sitemap XML]({baseUrl}/sitemap.xml)");

            return sb.ToString();
        });

        return Content(content ?? string.Empty, "text/plain", Encoding.UTF8);
    }

}
