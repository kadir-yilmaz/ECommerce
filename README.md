# E-Commerce

Bu proje, **Onion Architecture** prensipleri üzerine inşa edilmiş, ölçeklenebilir ve yüksek performanslı bir E-Ticaret backend sistemidir. Kurumsal mimari modelleri ve modern yazılım pratikleri (CQRS, MediatR, Serilog vb.) kullanılarak geliştirilmiştir.

## Mimari Yapı (Onion Architecture)

Proje, bağımlılıkların iç katmanlara doğru olduğu ve çekirdek (Core) mantığın dış dünyadan izole edildiği Onion mimarisine dayanmaktadır:

- **ECommerce.Domain**: Temel entity'ler ve value object'ler.
- **ECommerce.Application**: İş mantığı (Business Logic), CQRS modelleri, Handlers, Servis arayüzleri ve DTO'lar.
- **ECommerce.Infrastructure**: Dış servis entegrasyonları (Storage, Mail, Token vb.).
- **ECommerce.Persistence**: Veritabanı işlemleri (EF Core, SQL Server) ve Repository implementasyonları.
- **ECommerce.SignalR**: Gerçek zamanlı bildirimler ve hub yapıları.
- **ECommerce.WebAPI**: Uygulama giriş noktası, Controller'lar ve Middlewares.

## Teknik Özellikler ve Uygulanan Yaklaşımlar

Uygulama kapsamında aşağıdaki ileri seviye teknikler uygulanmıştır:

### CQRS & MediatR
Komut ve sorguların (Command/Query) birbirinden ayrılması sağlanarak uygulama karmaşıklığı azaltılmış ve performansı optimize edilmiştir. Tüm işlemler **MediatR** kütüphanesi üzerinden yürütülmektedir.

### 📝 Gelişmiş Loglama (Serilog)
**Serilog** entegrasyonu ile merkezi bir loglama yapısı kurulmuştur:
- **Hiyerarşik Kayıt**: Loglar; SQL Server, Seq, Console ve dosya bazlı (File) olarak birden fazla kanala aktarılır.
- **Özel Filtreleme**: `Serilog.Expressions` kullanılarak loglar hata seviyelerine (`errors/`), metod tiplerine (`http-get/`, `http-post/`) ve genel bilgilere (`general/`) göre otomatik olarak ayrıştırılır.

### Güvenlik ve Yetkilendirme (Identity & JWT)
- **Role-Based Auth**: Her bir Action için hassas yetkilendirme yapılabilen özel `RolePermissionFilter` filtresi.
- **JWT Authentication**: "Admin" şeması özelinde yapılandırılmış güvenli token yönetimi.
- **Secret Management**: Hassas veriler (Mail şifreleri, API keyleri) `.env` dosyaları ile dış dünyadan izole edilmiştir.

### Real-time Updates (SignalR)
Ürün eklendiğinde veya sipariş verildiğinde istemci tarafına anlık bildirimler gönderen Hub sistemleri entegre edilmiştir.

### Diğer Teknikler
- **Global Exception Handling**: Tüm uygulamadaki hataları merkezi olarak yakalayan ve loglayan middleware yapısı.
- **FluentValidation**: İstek modelleri için merkezi ve otomatik doğrulama sistemi.
- **Caching**: Performance optimizasyonu için **Redis** entegrasyonu.
- **Storage Abstraction**: Yerel veya bulut tabanlı (AWS, Azure vb.) depolama sağlayıcıları arasında kolayca geçiş yapmaya olanak tanıyan soyutlanmış yapı.

## Kurulum ve Çalıştırma

1. Projeyi klonlayın.
2. `ECommerce.WebAPI` dizini altında bir `.env` dosyası oluşturun ve gerekli değişkenleri tanımlayın (Örnek: `Mail__Password`, `Token__SecurityKey`).
3. `dotnet restore` komutu ile bağımlılıkları yükleyin.
4. `dotnet ef database update` ile veritabanı migration'larını tamamlayın.
5. Uygulamayı ayağa kaldırın: `dotnet run --project .\ECommerce.WebAPI\`.

---
