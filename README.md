# 💎 Akın Teknik — B2B Endüstriyel Kalıp & Parlatma Ekipmanları Katalog Platformu

[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-13.0-239120?logo=csharp&logoColor=white)](https://docs.microsoft.com/dotnet/csharp/)
[![EF Core 10](https://img.shields.io/badge/EF%20Core-10.0-blue)](https://docs.microsoft.com/ef/core/)
[![Tailwind CSS](https://img.shields.io/badge/Tailwind_CSS-3.4-38B2AC?logo=tailwind-css&logoColor=white)](https://tailwindcss.com/)
[![SQLite](https://img.shields.io/badge/SQLite-Zero_Config-003B57?logo=sqlite&logoColor=white)](https://sqlite.org/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Live Site](https://img.shields.io/badge/Canl%C4%B1%20Site-akinteknik.com-da291c?logo=googlechrome&logoColor=white)](https://akinteknik.com)

Endüstriyel kalıp sarf malzemeleri, polisaj ekipmanları, elmas pastalar ve teknik hırdavat tedarik süreçleri için geliştirilmiş; **sınırsız derinlikte hiyerarşik kategori ağacı**, **B2B teklif sepeti (RFQ)**, **WhatsApp hızlı sipariş/fiyat alma entegrasyonu** ve **otomatik WebP görsel işleme motoru** içeren modern web katalog platformu.

---

## 🌐 Orijinal Canlı Proje Referansı (Production Showcase)

> 🔗 **Canlı Web Sitesi:** [https://akinteknik.com](https://akinteknik.com)  
> 🏢 **Firma:** Akın Teknik Kalıpçılık ve Parlatma Ekipmanları San. Tic. Ltd. Şti. & Kuzey Parlatma  
> 👨‍💻 **Tasarım & Geliştirme:** [Orhan Efe Saçlı](https://www.orhanefesacli.com) (OES Yazılım)

Bu depo, canlı yayında aktif olarak hizmet veren [akinteknik.com](https://akinteknik.com) platformunun mimarisini, teknik çözümlerini ve kullanıcı deneyimini sergilemek amacıyla hazırlanmış **açık kaynaklı ve sanitize edilmiş bir demo sürümüdür**.

### 🛡️ Gizlilik & Demo Uyarlaması
* **Veritabanı & Altyapı:** Canlı MySQL veritabanı ve müşteri teklif verileri tamamen arındırılmış, yerine sıfır konfigürasyonla çalışan **tak-çalıştır SQLite** entegre edilmiştir.
* **Görseller:** Telif ve ticari gizlilik sebebiyle demo projedeki ürün ve vitrin fotoğrafları **özel endüstriyel yapay zeka fotoğrafçılığı** ile sıfırdan oluşturulmuştur.
* **İletişim & Konum:** Gerçek şirket telefon numaraları, şirket içi e-postalar ve harita pini generic/demo verilerle maskelenmiştir.
* **Canlı vs Demo Karşılaştırması:** Sistemin gerçek ürün ağacını, tam kurumsal kimliğini ve canlı operasyonunu görmek için [akinteknik.com](https://akinteknik.com) adresini ziyaret edebilirsiniz.

---

## 🌟 Öne Çıkan Özellikler

* 🌳 **Çok Kademeli (Multi-Level) Kategori Ağacı:** 
  Sınırsız derinlikte alt ve torun kategorileri (*Örn: ELMAS PASTALAR > NACTA ELMAS PASTALAR > NACTA - 40 GRAM*) birbirine bağlayan in-memory hiyerarşi mimarisi. Seçili kategorinin tüm üst dallarını otomatik açan (auto-expand) ve alt dalları listeleyen akıllı sidebar filtreleme sistemi.
* ⚡ **Tak-Çalıştır SQLite Desteği (Zero-Configuration):**
  Depoyu klonlayan herkes hiçbir MySQL, PostgreSQL veya harici sunucu kurmadan projeyi tek tıkla çalıştırabilir. İlk çalıştırmada SQLite tabloları ve zengin demo verileri (`DbInitializer`) otomatik oluşturulur.
* 📄 **B2B Teklif & WhatsApp Entegrasyonu:**
  Klasik e-ticaret ödemesi yerine kurumsal alımlara uygun **Teklif Listesi (RFQ)** ve ürün sayfasından seçili varyantla doğrudan yetkiliye bağlanan WhatsApp hızlı fiyat alma butonu.
* 🖼️ **Otomatik WebP Görsel Optimizasyonu:**
  Admin panelinden yüklenen ürün ve kategori fotoğraflarını arka planda otomatik olarak boyutlandıran, sıkıştıran ve modern WebP formatına dönüştüren `SixLabors.ImageSharp` motoru.
* 📊 **Varyant & Teknik Özellik Yönetimi:**
  Her ürün için farklı ölçü, mikron boyutu, sertlik derecesi ve tip tanımlayabilme; varyanta özel fotoğraf ve fiyat eşleştirme.
* 🔐 **Hafif & Güvenli Yönetici Paneli:**
  Cookie Authentication ile korunan yönetim paneli: Ürünler, çok kademeli kategoriler, slaytlar, kurumsal içerik ve blog yönetimi.
* 🚀 **Yüksek Performans & SEO:**
  Dinamik XML Sitemap (`/sitemap.xml`), `robots.txt`, yapay zeka arama motorları için `llms.txt`, Google Rich Snippets uyumlu Schema.org (LocalBusiness & Product JSON-LD) mikroveri altyapısı ve güvenlik başlıkları (Security Headers).

---

## 🚀 Hızlı Başlangıç (Quick Start)

Projeyi bilgisayarınızda çalıştırmak için yalnızca [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)'nın kurulu olması yeterlidir.

### 1. Depoyu Klonlayın
```bash
git clone https://github.com/KULLANICI_ADINIZ/akinteknik-demo.git
cd akinteknik-demo
```

### 2. Projeyi Çalıştırın
```bash
dotnet run
```
*veya hot-reload ile çalıştırmak için:*
```bash
dotnet watch
```

Tarayıcınızda şu adresi açın:
👉 **`http://localhost:5272`** *(veya SSL ile `https://localhost:7105`)*

> 💡 **Not:** İlk çalıştırmada SQLite veritabanı (`akinteknik_demo.db`), tablolar ve örnek demo ürünleri otomatik olarak oluşturulur. Ek bir ayar yapmanıza gerek yoktur.

---

## 🔑 Demo Yönetici (Admin) Giriş Bilgileri

Yönetim paneline erişmek için:

* **Giriş URL'i:** `http://localhost:5272/Admin/Account/Login`
* **Kullanıcı Adı:** `admin`
* **Şifre:** `admin123`

---

## 🗄️ Veritabanı Seçenekleri

Varsayılan olarak **SQLite** kullanılmaktadır. İsteğe bağlı olarak **MySQL / MariaDB** kullanmak isterseniz `appsettings.json` dosyasındaki `ConnectionStrings:DefaultConnection` değerini MySQL bağlantı adresinizle değiştirmeniz yeterlidir:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=akinteknik_demo;User=root;Password=YourPassword;CharSet=utf8mb4;"
  }
}
```

---

## 🛠️ Teknoloji Yığını (Tech Stack)

| Alan | Teknoloji | Açıklama |
|---|---|---|
| **Framework** | ASP.NET Core 10 (MVC) | Modern, yüksek performanslı web çatısı |
| **Dil** | C# 13 | Son sürüm dil özellikleri |
| **ORM** | Entity Framework Core 10 | Code-First / DbContext veri erişimi |
| **Veritabanı** | SQLite (Demo) / MySQL (Canlı) | Sıfır konfigürasyon & taşınabilir veritabanı |
| **Stil & Tasarım** | Tailwind CSS 3.4 | Modern, utility-first CSS |
| **İkonlar** | FontAwesome 6 | Vektörel ikon kütüphanesi |
| **Scripting** | Vanilla JavaScript | Sıfır harici JS şişkinliği, yüksek hız |
| **Görsel İşleme** | SixLabors ImageSharp 4.1.3 | Otomatik WebP dönüştürücü ve ölçekleyici |

---

## 📂 Proje Dizin Yapısı

```text
├── Areas/
│   └── Admin/              # Yönetim Paneli (Controllers & Views)
├── Controllers/            # Web Vitrini Kontrolcüleri (Home, Product, Category, Cart, Blog)
├── Data/
│   ├── ApplicationDbContext.cs
│   └── DbInitializer.cs    # Otomatik SQLite demo veri tohumlayıcı
├── Helpers/
│   ├── CategoryHelper.cs   # Çok kademeli özyinelemeli kategori algoritması
│   └── ImageHelper.cs      # WebP dönüştürücü ve görsel optimizasyonu
├── Models/                 # Veritabanı Entity ve ViewModel sınıfları
├── Properties/
│   └── launchSettings.json # Yerel geliştirme port ve profil ayarları (Port 5272/7105)
├── Views/                  # Razor Görünüm Şablonları
│   └── Shared/
│       └── _CategoryFilterNode.cshtml  # Özyinelemeli (Recursive) Sidebar Bileşeni
├── wwwroot/                # Statik Dosyalar (CSS, JS, Resimler)
├── appsettings.json        # Demo ayar dosyası (Hassas veri içermez)
├── AkinTeknik.Demo.csproj  # .NET Proje Tanımı
└── README.md
```

---

## 👨‍💻 Geliştirici & İletişim

* **Geliştirici:** Orhan Efe Saçlı
* **Web Sitesi:** [https://www.orhanefesacli.com](https://www.orhanefesacli.com)
* **LinkedIn:** [Orhan Efe Saçlı](https://linkedin.com)

---

## 📄 Lisans

Bu proje [MIT Lisansı](LICENSE) kapsamında açık kaynak olarak sunulmaktadır.
İnceleyebilir, fork'layabilir ve kendi B2B projelerinizde serbestçe kullanabilirsiniz.
