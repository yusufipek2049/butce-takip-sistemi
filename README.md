# Bütçe Takip Sistemi

Bütçe Takip Sistemi, kişisel gelir ve giderleri düzenli takip etmek için hazırlanmış sade bir ASP.NET Core web uygulamasıdır. Kullanıcı gelirlerini, harcamalarını, kategorilerini ve aylık bütçe hedeflerini tek ekranda yönetebilir. Dashboard tarafında aylık özetler, kategori bazlı gider dağılımı, güncel bakiye ve bütçe uyarıları birlikte gösterilir.

Proje Razor Pages, C#, Entity Framework Core ve SQLite ile geliştirildi. Kapsam özellikle SRS dokümanındaki FR-01 ile FR-10 arasındaki gereksinimlere göre sınırlandırıldı. Bu yüzden uygulamada kimlik doğrulama, banka API bağlantısı veya çok kullanıcılı abonelik yapısı yoktur. Odak tamamen temel bütçe takip akışlarıdır.

Ana proje dosyası `ButceTakipSistemi.csproj`, yerel veritabanı dosyası ise `butce-takip-sistemi.db` adını kullanır.

## Kullanılan Teknolojiler

- ASP.NET Core 8 Razor Pages
- C#
- Entity Framework Core 8
- SQLite
- Bootstrap 5
- DataAnnotations ile form doğrulama
- ViewComponent ile layout üzerinde güncel bakiye gösterimi

## Proje Yapısı

```text
Data/            DbContext, migration başlangıcı ve seed verileri
Models/          Transaction, Category, BudgetGoal ve enum modelleri
Services/        Dashboard, bakiye, işlem, kategori ve bütçe servisleri
ViewComponents/  Header içindeki güncel bakiye bileşeni
ViewModels/      Form ve listeleme ekranları için kullanılan modeller
Pages/           Razor Pages ekranları ve PageModel sınıfları
Migrations/      EF Core migration dosyaları
```

Razor dosyaları ağırlıklı olarak arayüzden sorumludur. Hesaplama, veri erişimi ve iş akışı servisler ile PageModel sınıflarında tutulur. Dashboard hesaplarında LINQ `Where`, `GroupBy`, `Sum` ve `Select` sorguları kullanılır.

## Kurulum

Projeyi ilk kez çalıştırmadan önce bağımlılıkları yükleyin:

```bash
dotnet restore
dotnet tool restore
```

## Veritabanı

İlk migration hazır durumdadır. SQLite veritabanını oluşturmak veya mevcut veritabanını güncellemek için:

```bash
dotnet tool run dotnet-ef database update
```

Uygulama açılırken `DbInitializer` çalışır. Veritabanı boşsa örnek gelir/gider kategorileri, bütçe hedefleri ve birkaç demo işlem otomatik eklenir.

Yeni migration eklemek gerekirse:

```bash
dotnet tool run dotnet-ef migrations add MigrationAdi
dotnet tool run dotnet-ef database update
```

## Çalıştırma

```bash
dotnet run
```

Komuttan sonra terminalde görünen yerel adresi tarayıcıda açabilirsiniz. Örneğin:

```text
http://localhost:5000
```

## Kontrol Edilecek Akışlar

Uygulamayı hızlıca test etmek için şu akışlara bakabilirsiniz:

1. Dashboard ekranında ay/yıl filtresiyle toplam gelir, toplam gider, net bakiye ve kategori bazlı gider özetini kontrol edin.
2. Gelir ekleyin. Tutar 0 veya negatif girildiğinde formun hata verdiğini doğrulayın.
3. Gider ekleyin. İlgili bütçe limitine yaklaşıldığında veya limit aşıldığında uyarı mesajının göründüğünü kontrol edin.
4. İşlemler sayfasında kayıt düzenleme ve silme adımlarını deneyin.
5. Kategori yönetiminde aynı türde aynı kategori adının tekrar eklenmediğini kontrol edin.
6. Kullanılmış kategorilerin silinmek yerine pasifleştirildiğini doğrulayın.
7. Header alanındaki güncel bakiyenin sayfalar arasında güncel kaldığını kontrol edin.

Temel teknik doğrulama için:

```bash
dotnet build
dotnet tool run dotnet-ef database update
```

## SRS Kontrol Listesi

- FR-01 Dashboard görüntüleme: Ay/yıl filtresi, aylık gelir/gider/net bakiye, kategori özeti ve bütçe durumları gösteriliyor.
- FR-02 Gelir kaydı ekleme: Pozitif tutar kuralı ve gelir kategorisi seçimi uygulanıyor.
- FR-03 Gider kaydı ekleme: Pozitif tutar kuralı, gider kategorisi seçimi ve bütçe uyarısı akışı var.
- FR-04 Kategori yönetimi: Gelir/gider kategorileri ekleniyor, listeleniyor ve pasifleştirilebiliyor.
- FR-05 Bütçe hedefi tanımlama: Gider kategorisi, ay, yıl ve limit tutarı ile hedef kaydediliyor.
- FR-06 Limit aşımı uyarısı: Dashboard ve gider ekleme sonrası limit aşımı/yaklaşma uyarıları gösteriliyor.
- FR-07 LINQ agregasyonları: Dashboard hesapları servis katmanında LINQ sorgularıyla üretiliyor.
- FR-08 Header bakiye gösterimi: `_Layout.cshtml` içinde `BalanceViewComponent` ile güncel bakiye gösteriliyor.
- FR-09 Kayıt düzenleme/silme: İşlemler sayfasından kayıt düzenleme ve silme akışları çalışıyor.
- FR-10 Form doğrulama: DataAnnotations, server-side validation, client-side validation ve anti-forgery token kullanılıyor.
