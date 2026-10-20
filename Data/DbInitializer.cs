using System;
using System.Collections.Generic;
using System.Linq;
using AkinTeknik.Web.Models;

namespace AkinTeknik.Web.Data;

public static class DbInitializer
{
    public static void SeedDemoData(ApplicationDbContext context)
    {
        // Veritabanı zaten doluysa tekrar ekleme yapma
        if (context.Categories.Any() || context.Products.Any())
        {
            return;
        }

        // 1. Sliders (Slaytlar)
        var sliders = new List<Slider>
        {
            new Slider
            {
                ImageUrl = "/images/sliders/slider-1.jpg",
                TargetUrl = "/Category",
                DisplayOrder = 1
            },
            new Slider
            {
                ImageUrl = "/images/sliders/slider-2.jpg",
                TargetUrl = "/Product",
                DisplayOrder = 2
            }
        };
        context.Sliders.AddRange(sliders);
        context.SaveChanges();

        // 2. Hakkımızda (About)
        var about = new About
        {
            Title = "Akın Teknik Endüstriyel Kalıp Sarf Malzemeleri",
            SubTitle = "Yüksek Kalite & Güvenilir Tedarik",
            Content = "Akın Teknik ve tescilli markamız KUZEY ile; endüstriyel kalıp polisaj sarf malzemeleri, elmas pastalar, parlatma keçeleri ve teknik eğelerde toptan ve perakende çözüm ortağınızız. Türkiye genelinde kalıp sanayi ve imalat atölyelerine doğrudan stoktan hızlı gönderim sağlıyoruz.",
            ImageUrl = "/images/kuzey.jpg",
            Mission = "Kalıp ve imalat sektörüne dünya standartlarında, yüksek aşındırma ve parlatma verimliliği sunan ürünleri uygun maliyet ve kesintisiz stokla sunmak.",
            Vision = "Endüstriyel parlatma ve kalıp ekipmanları alanında Türkiye'nin en çok tercih edilen güvenilir teknik tedarik markası olmak.",
            ExperienceYears = "15+",
            ProductCount = "500+",
            CustomerCount = "1200+",
            SatisfactionRate = "%99"
        };
        context.Abouts.Add(about);
        context.SaveChanges();

        // 3. Çok Kademeli Kategoriler (Multi-Level Categories)
        // Kök Kategoriler (Level 1)
        var catElmasPastalar = new Category
        {
            Name = "ELMAS PASTALAR",
            Slug = "elmas-pastalar",
            Description = "Hassas kalıp parlatma ve yüzey finişleme için yüksek konsantrasyonlu elmas pastalar.",
            ImageUrl = "/images/categories/cat-elmas-pasta.jpg",
            DisplayOrder = 1,
            IsFeatured = true
        };

        var catKeceler = new Category
        {
            Name = "PARLATMA KEÇELERİ",
            Slug = "parlatma-keceleri",
            Description = "Yüksek yoğunluklu saf yün parlatma keçeleri, silindirik ve konik çeşitler.",
            ImageUrl = "/images/categories/cat-parlatma-kece.jpg",
            DisplayOrder = 2,
            IsFeatured = true
        };

        var catEgeler = new Category
        {
            Name = "ELMAS EĞELER",
            Slug = "elmas-egeler",
            Description = "Sertleştirilmiş çelik ve karbür kalıp işleme elmas eğe çeşitleri.",
            ImageUrl = "/images/categories/cat-elmas-ege.jpg",
            DisplayOrder = 3,
            IsFeatured = true
        };

        context.Categories.AddRange(catElmasPastalar, catKeceler, catEgeler);
        context.SaveChanges();

        // 2. Kademe Kategoriler (Level 2 - Alt Kategoriler)
        var subNorthDiamond = new Category
        {
            Name = "NORTH DIAMOND ELMAS PASTALAR",
            Slug = "north-diamond-elmas-pastalar",
            Description = "North Diamond profesyonel kalıp parlatma pastaları.",
            ImageUrl = "/images/categories/cat-elmas-pasta.jpg",
            ParentCategoryId = catElmasPastalar.Id,
            DisplayOrder = 1
        };

        var subNacta = new Category
        {
            Name = "NACTA ELMAS PASTALAR",
            Slug = "nacta-elmas-pastalar",
            Description = "Nacta mikronize yüzey işleme ve ayna parlatma pastaları.",
            ImageUrl = "/images/categories/cat-elmas-pasta.jpg",
            ParentCategoryId = catElmasPastalar.Id,
            DisplayOrder = 2
        };

        var subVidaliKeceler = new Category
        {
            Name = "Vidalı Silindirik Keçeler",
            Slug = "vidali-silindirik-keceler",
            Description = "Vidalı montajlı silindirik parlatma keçeleri.",
            ImageUrl = "/images/categories/cat-parlatma-kece.jpg",
            ParentCategoryId = catKeceler.Id,
            DisplayOrder = 1
        };

        var subDuzEgeler = new Category
        {
            Name = "Düz Elmas Eğeler",
            Slug = "duz-elmas-egeler",
            Description = "Düz formlu yüksek dayanımlı elmas eğeler.",
            ImageUrl = "/images/categories/cat-elmas-ege.jpg",
            ParentCategoryId = catEgeler.Id,
            DisplayOrder = 1
        };

        context.Categories.AddRange(subNorthDiamond, subNacta, subVidaliKeceler, subDuzEgeler);
        context.SaveChanges();

        // 3. Kademe Kategoriler (Level 3 - Torun Kategoriler)
        var subNorth20g = new Category
        {
            Name = "NORTH DIAMOND - 20 GRAM",
            Slug = "north-diamond-20-gram",
            Description = "North Diamond 20 gram şırınga ambalajlı elmas pastalar.",
            ImageUrl = "/images/products/prod-north-20g.jpg",
            ParentCategoryId = subNorthDiamond.Id,
            DisplayOrder = 1
        };

        var subNorth40g = new Category
        {
            Name = "NORTH DIAMOND - 40 GRAM",
            Slug = "north-diamond-40-gram",
            Description = "North Diamond 40 gram tüp ambalajlı elmas pastalar.",
            ImageUrl = "/images/categories/cat-elmas-pasta.jpg",
            ParentCategoryId = subNorthDiamond.Id,
            DisplayOrder = 2
        };

        var subNorth500g = new Category
        {
            Name = "NORTH DIAMOND - 500 GRAM",
            Slug = "north-diamond-500-gram",
            Description = "North Diamond 500 gram endüstriyel kavanoz elmas pastalar.",
            ImageUrl = "/images/categories/cat-elmas-pasta.jpg",
            ParentCategoryId = subNorthDiamond.Id,
            DisplayOrder = 3
        };

        var subNacta40g = new Category
        {
            Name = "NACTA - 40 GRAM",
            Slug = "nacta-40-gram",
            Description = "Nacta 40 gram yüksek konsantrasyonlu elmas parlatma pastaları.",
            ImageUrl = "/images/products/prod-nacta-3mikron.jpg",
            ParentCategoryId = subNacta.Id,
            DisplayOrder = 1
        };

        var subNacta500g = new Category
        {
            Name = "NACTA - 500 GRAM",
            Slug = "nacta-500-gram",
            Description = "Nacta 500 gram atölye tipi ekonomik elmas pasta.",
            ImageUrl = "/images/categories/cat-elmas-pasta.jpg",
            ParentCategoryId = subNacta.Id,
            DisplayOrder = 2
        };

        context.Categories.AddRange(subNorth20g, subNorth40g, subNorth500g, subNacta40g, subNacta500g);
        context.SaveChanges();

        // 4. Örnek Ürünler (Products)
        var p1 = new Product
        {
            Name = "Nacta Elmas Pasta 40 Gr (3 Mikron - Sarı)",
            Slug = "nacta-elmas-pasta-40-gr-3-mikron",
            CategoryId = subNacta40g.Id,
            Price = 450.00m,
            Stock = 50,
            ImageUrl = "/images/products/prod-nacta-3mikron.jpg",
            Dimensions = "40 Gram",
            Type = "Yağ Bazlı",
            Detail = "3 Mikron (Ön Polisaj)",
            UsageArea = "Kalıp parlatma, enjeksiyon kalıpları, sert metal yüzeyler",
            Description = "Nacta Elmas Parlatma Pastası 40g, hassas kalıp yüzeylerinde ayna parlaklığına geçiş öncesi pürüzsüzleştirme sağlar.\n\nYüksek konsantrasyonda sentetik elmas tozu içerir. Çelik, karbür ve seramik parçalarda keçeyle birlikte mükemmel sonuç verir.",
            IsBestSeller = true
        };

        var p2 = new Product
        {
            Name = "Nacta Elmas Pasta 40 Gr (1 Mikron - Beyaz)",
            Slug = "nacta-elmas-pasta-40-gr-1-mikron",
            CategoryId = subNacta40g.Id,
            Price = 475.00m,
            Stock = 40,
            ImageUrl = "/images/products/prod-nacta-1mikron.jpg",
            Dimensions = "40 Gram",
            Type = "Yağ Bazlı",
            Detail = "1 Mikron (Ayna Parlaklığı)",
            UsageArea = "Son finiş parlatma, optik kalıplar, ayna parlaklığı",
            Description = "1 Mikron süper ince elmas tozu ile nihai ayna parlaklığı (mirror finish) elde etmek için üretilmiştir.\n\nKalıp temizliği ve finiş parlatmada endüstri standardı kalitededir.",
            IsBestSeller = true
        };

        var p3 = new Product
        {
            Name = "North Diamond 20 Gr Şırınga Elmas Pasta (6 Mikron)",
            Slug = "north-diamond-20-gr-sirinya-elmas-pasta",
            CategoryId = subNorth20g.Id,
            Price = 280.00m,
            Stock = 75,
            ImageUrl = "/images/products/prod-north-20g.jpg",
            Dimensions = "20 Gram Şırınga",
            Type = "Kolay Uygulama",
            Detail = "6 Mikron (Orta Aşındırma)",
            UsageArea = "Plastik enjeksiyon kalıpları, kesme kalıpları",
            Description = "North Diamond 20 gram pratik dozajlama şırıngası sayesinde firesiz ve temiz kullanım imkanı sağlar.\n\nYüksek kaliteli elmas partikülleri ile hızlı talaş kaldırma ve yüzey düzeltme sağlar.",
            IsBestSeller = false
        };

        var p4 = new Product
        {
            Name = "Kuzey Vidalı Silindirik Parlatma Keçesi",
            Slug = "kuzey-vidali-silindirik-parlatma-kecesi",
            CategoryId = subVidaliKeceler.Id,
            Price = 45.00m,
            Stock = 500,
            ImageUrl = "/images/products/prod-kece-silindirik.jpg",
            Dimensions = "25x30 mm",
            Type = "Vidalı Şaft",
            Detail = "Yüksek Yoğunluk Saf Yün",
            UsageArea = "Kalıp içi polisaj, el motorları, havalı kalıpçı taşlama",
            Description = "Kuzey tescilli markalı vidalı parlatma keçeleri; yüksek yoğunluklu preslenmiş yünden imal edilmiştir.\n\nElmas pasta veya parlatma cilaları ile birlikte yüksek devirde dağılma yapmadan uzun ömürlü çalışır.",
            IsBestSeller = true
        };

        var p5 = new Product
        {
            Name = "Kuzey Profesyonel Düz Elmas Eğe 200 mm",
            Slug = "kuzey-profesyonel-duz-elmas-ege-200-mm",
            CategoryId = subDuzEgeler.Id,
            Price = 220.00m,
            Stock = 120,
            ImageUrl = "/images/products/prod-elmas-ege-200.jpg",
            Dimensions = "200 mm / Düz",
            Type = "#150 Kum",
            Detail = "Ergonomik Saplı",
            UsageArea = "Kalıp çelikleri, sertleştirilmiş metaller, cam ve seramik",
            Description = "Kuzey elmas eğeler, elektro-kaplama teknolojisi ile endüstriyel elmas partikülleri çelik gövdeye sabitlenerek üretilmiştir.\n\nSertleştirilmiş kalıp çeliklerinde aşınmadan uzun süre keskinliğini korur.",
            IsBestSeller = true
        };

        context.Products.AddRange(p1, p2, p3, p4, p5);
        context.SaveChanges();

        // 5. Varyantlar (ProductSizes)
        var sizes = new List<ProductSize>
        {
            new ProductSize { ProductId = p4.Id, Size = "15x20 mm", Price = 35.00m, Stock = 200, Type = "Vidalı" },
            new ProductSize { ProductId = p4.Id, Size = "20x25 mm", Price = 40.00m, Stock = 200, Type = "Vidalı" },
            new ProductSize { ProductId = p4.Id, Size = "25x30 mm", Price = 45.00m, Stock = 150, Type = "Vidalı" },
            new ProductSize { ProductId = p4.Id, Size = "30x35 mm", Price = 55.00m, Stock = 100, Type = "Vidalı" },

            new ProductSize { ProductId = p5.Id, Size = "160 mm - #150 Kum", Price = 190.00m, Stock = 50, Type = "Düz" },
            new ProductSize { ProductId = p5.Id, Size = "200 mm - #150 Kum", Price = 220.00m, Stock = 80, Type = "Düz" },
            new ProductSize { ProductId = p5.Id, Size = "200 mm - #200 Kum (İnce)", Price = 230.00m, Stock = 60, Type = "Düz İnce" }
        };
        context.ProductSizes.AddRange(sizes);
        context.SaveChanges();

        // 6. Blog Yazıları
        var blogs = new List<BlogPost>
        {
            new BlogPost
            {
                Title = "Kalıp Parlatmada Elmas Pasta Seçimi Nasıl Yapılır?",
                Slug = "kalip-parlatmada-elmas-pasta-secimi",
                Summary = "Enjeksiyon ve kesme kalıplarında doğru mikron seçimi, parlatma süresini yarı yarıya azaltır.",
                Content = "Kalıp polisajında başarı; yüzeydeki pürüzlülük derecesine uygun tane boyutunu sırayla kullanmaktan geçer. Kaba aşındırmada 15-25 mikron, ara yüzey geçişlerinde 6-9 mikron, nihai ayna parlaklığında ise 1-3 mikron elmas pastalar tercih edilmelidir...",
                ImageUrl = "/images/categories/cat-elmas-pasta.jpg",
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                IsPublished = true
            },
            new BlogPost
            {
                Title = "Yüksek Yoğunluklu Parlatma Keçelerinin Kalıpçılıktaki Önemi",
                Slug = "parlatma-kecelerinin-kalipcilikteki-onemi",
                Summary = "Doğal yün keçelerin esneklik ve ısı dağılımı konusundaki avantajları.",
                Content = "Preslenmiş saf yün keçeler, çalışma esnasında sürtünmeden kaynaklanan ısıyı kalıp yüzeyine homojen dağıtarak elmas pastanın etkinliğini maksimize eder. Sentetik keçelere kıyasla yüzeyde çizik bırakmaz ve form bozulması yaşanmaz...",
                ImageUrl = "/images/categories/cat-parlatma-kece.jpg",
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                IsPublished = true
            }
        };
        context.BlogPosts.AddRange(blogs);
        context.SaveChanges();
    }
}
