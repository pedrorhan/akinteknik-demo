using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Webp;

namespace AkinTeknik.Web.Helpers;

public static class ImageHelper
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp", ".jfif", ".bmp"
    };

    private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB limit

    public static async Task<string?> SaveAsWebpAsync(IFormFile? file, string webRootPath, string folderName, int maxWidth = 1200, int quality = 75)
    {
        if (file == null || file.Length == 0) return null;

        // 1. Dosya boyutu kontrolü (DoS ve OOM önlemi)
        if (file.Length > MaxFileSizeBytes)
        {
            return null;
        }

        // 2. Uzantı beyaz liste kontrolü
        var ext = Path.GetExtension(file.FileName);
        if (string.IsNullOrEmpty(ext) || !AllowedExtensions.Contains(ext))
        {
            return null;
        }

        string uploadsFolder = Path.Combine(webRootPath, "uploads", folderName);
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        string fileName = $"{Guid.NewGuid()}.webp";
        string filePath = Path.Combine(uploadsFolder, fileName);

        try
        {
            // 3. Dosyayı güvenli şekilde çözümle ve WebP'ye dönüştür
            using (var stream = file.OpenReadStream())
            using (var image = await Image.LoadAsync(stream))
            {
                if (image.Width > maxWidth)
                {
                    int newHeight = (int)((double)image.Height / image.Width * maxWidth);
                    image.Mutate(x => x.Resize(maxWidth, newHeight));
                }

                var encoder = new WebpEncoder
                {
                    Quality = quality
                };

                await image.SaveAsync(filePath, encoder);
            }

            return $"/uploads/{folderName}/{fileName}";
        }
        catch
        {
            // Bozuk veya sahte resim durumunda güvenli şekilde null dön
            return null;
        }
    }
}