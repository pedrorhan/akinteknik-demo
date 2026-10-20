# 💎 Akın Teknik — B2B Endüstriyel Kalıp & Parlatma Ekipmanları Katalog Platformu

[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-13.0-239120?logo=csharp&logoColor=white)](https://docs.microsoft.com/dotnet/csharp/)
[![EF Core 10](https://img.shields.io/badge/EF%20Core-10.0-blue)](https://docs.microsoft.com/ef/core/)
[![Tailwind CSS](https://img.shields.io/badge/Tailwind_CSS-3.4-38B2AC?logo=tailwind-css&logoColor=white)](https://tailwindcss.com/)
[![SQLite](https://img.shields.io/badge/SQLite-Zero_Config-003B57?logo=sqlite&logoColor=white)](https://sqlite.org/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

Endüstriyel kalıp sarf malzemeleri, polisaj ekipmanları, elmas pastalar ve teknik hırdavat tedarik süreçleri için geliştirilmiş; **sınırsız derinlikte hiyerarşik kategori ağacı**, **B2B teklif sepeti (RFQ)**, **WhatsApp sipariş entegrasyonu** ve **WebP görsel optimizasyonu** içeren modern web katalog platformu.

---

## 🌟 Öne Çıkan Özellikler

* 🌳 **Çok Kademeli (Multi-Level) Kategori Ağacı:** 
  Sınırsız derinlikte alt ve torun kategorileri (*Örn: ELMAS PASTALAR > NACTA ELMAS PASTALAR > NACTA - 40 GRAM*) birbirine bağlayan in-memory hiyerarşi mimarisi. Seçili kategorinin tüm üst dallarını otomatik açan (auto-expand) akıllı sidebar filtreleme sistemi.
* ⚡ **Tak-Çalıştır SQLite Desteği (Zero-Configuration):**
  GitHub'dan indiren herkes hiçbir MySQL veya harici veritabanı kurmadan projeyi tek tıkla çalıştırabilir. İlk çalıştırmada SQLite tabloları ve zengin demo verileri (`DbInitializer`) otomatik oluşturulur.
* 📄 **B2B Teklif & WhatsApp Entegrasyonu:**
  Klasik e-ticaret ödemesi yerine kurumsal alımlara uygun **Teklif Listesi (RFQ)** ve ürün sayfasından seçili varyantla doğrudan yetkiliye bağlanan WhatsApp hızlı fiyat alma butonu.
* 🖼️ **Otomatik WebP Görsel Optimizasyonu:**
  Admin panelinden yüklenen ürün ve kategori fotoğraflarını arka planda otomatik olarak boyutlandıran ve modern WebP formatına dönüştüren `SixLabors.ImageSharp` motoru.
* 📊 **Varyant & Teknik Özellik Yönetimi:**
  Her ürün için farklı ölçü, mikron boyutu, sertlik derecesi ve tip tanımlayabilme; varyanta özel fotoğraf ve fiyat eşleştirme.
* 🔐 **Hafif & Güvenli Yönetici Paneli:**
  Cookie Authentication ile korunan yönetim paneli: Ürünler, çok kademeli kategoriler, slaytlar, kurumsal içerik ve blog yönetimi.
* 🚀 **Yüksek Performans & SEO:**
  Sıfır harici JS bağımlılığı (Vanilla JS), hızlı yanıt veren responsive Tailwind CSS tasarımı ve Google Rich Snippets uyumlu Schema.org mikroveri JSON-LD altyapısı.

---

## 🚀 Hızlı Başlangıç (Quick Start)

Projeyi bilgisayarınızda çalıştırmak için yalnızca [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)'nın kurulu olması yeterlidir.

### 1. Depoyu Klonlayın
```bash
git clone https://github.com/KULLANICI_ADINIZ/akinteknik-b2b-catalog.git
cd akinteknik-b2b-catalog
```

### 2. Projeyi Çalıştırın
```bash
dotnet run
```

Konsolda verilen adresi tarayıcınızda açın (Genellikle: `https://localhost:5001` veya `http://localhost:5000`).

> 💡 **Not:** İlk çalıştırmada SQLite veritabanı (`akinteknik_demo.db`), tablolar ve örnek demo ürünleri otomatik olarak oluşturulur. Ek bir işlem yapmanıza gerek yoktur.

---

## 🔑 Demo Yönetici (Admin) Giriş Bilgileri

Yönetim paneline erişmek için:

* **Giriş URL'i:** `https://localhost:5001/Admin/Account/Login`
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

| Alan | Teknoloji |
|---|---|
| **Framework** | ASP.NET Core 10 (MVC) |
| **Dil** | C# 13 |
| **ORM** | Entity Framework Core 10 |
| **Veritabanı** | SQLite (Varsayılan Demo) / MySQL & MariaDB |
| **Stil & Tasarım** | Tailwind CSS 3.4 |
| **İkonlar** | FontAwesome 6 |
| **Scripting** | Vanilla JavaScript (Sıfır ek kütüphane) |
| **Görsel İşleme** | SixLabors ImageSharp 4.1.3 |

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
├── Views/                  # Razor Görünüm Şablonları
│   └── Shared/
│       └── _CategoryFilterNode.cshtml  # Özyinelemeli (Recursive) Sidebar Bileşeni
├── wwwroot/                # Statik Dosyalar (CSS, JS, Resimler)
├── appsettings.json        # Demo ayar dosyası (Hassas veri içermez)
├── AkinTeknik.Demo.csproj  # .NET Proje Tanımı
└── README.md
```

---

## 🛡️ Güvenlik ve Gizlilik Bildirimi

Bu açık kaynak demo projesi portföy ve eğitim amaçlı hazırlanmıştır:
* Gerçek canlı sunucu şifreleri, API anahtarları veya veritabanı bilgileri içermez.
* Tüm iletişim bilgileri, telefon numaraları ve e-postalar demo verilerle maskelenmiştir.

---

## 📄 Lisans

Bu proje [MIT Lisansı](LICENSE) kapsamında açık kaynak olarak sunulmaktadır.
