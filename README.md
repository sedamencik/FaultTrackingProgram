# 🛠️ Fault Tracking System (Arıza Takip Sistemi)
Bu proje, kurumsal ortamlarda teknik arıza bildirimlerini yönetmek, takip etmek ve durumlarını profesyonel bir iş akışıyla (State Machine) güncellemek için geliştirilmiş ASP.NET Core 8 Web API tabanlı bir sistemdir.

## 🚀 Projeyi Çalıştırma Adımları
Projeyi çalıştırmak için bilgisayarınızda Docker ve Docker Desktop kurulu olmalıdır.

1. Projeyi Klonlayın

Bash

git clone https://github.com/sedamencik/FaultTrackingProgram.git

2. Sistemi Ayağa Kaldırın (Temiz Kurulum)

Aşağıdaki komut; SQL Server ve Web API konteynerlarını yapılandırır, bağımlılıkları yükler ve sistemi başlatır. 

Bash

docker compose down -v

docker compose up -d --build

3. Veritabanı ve Erişim

Konteynerlar ayağa kalktığında API, SQL Server'a otomatik olarak bağlanır.

Swagger UI: http://localhost:5005/swagger

API Base URL: http://localhost:5005/api

### 🏗️ Kullanılan Teknolojiler ve Kütüphaneler
Framework: .NET 8.0 (Web API)

Database: Microsoft SQL Server

ORM: Entity Framework Core (Code First)

Mapping: AutoMapper (Entity-DTO dönüşümleri)

Logging: Serilog (Konsol ve dosya tabanlı structured logging)

Containerization: Docker & Docker Compose

Documentation: Swagger / OpenAPI

### 🔄 Durum Makinesi (State Machine) Akışı
Sistemdeki arıza bildirimleri kontrolsüz bir şekilde durum değiştiremez. Veri bütünlüğünü korumak adına aşağıdaki mantıksal akış uygulanmıştır:

YeniKayit: Her bildirim bu statüde başlar.

Inceleniyor: Sadece YeniKayit olan bildirimler bu aşamaya geçebilir.

Atandi / Calisiliyor: İnceleme sonrası teknik ekibe yönlendirme aşaması.

Tamamlandi: Sadece Calisiliyor durumundaki bir kayıt başarıyla kapatılabilir.

Iptal / Asilsiz: Kayıt, tamamlanmadığı sürece herhangi bir aşamada iptal edilebilir.

[!IMPORTANT]
Geçersiz bir durum değişikliği isteğinde (örn: Yeni kaydı direkt tamamlandıya çekmek) API 422 Unprocessable Entity hatası döner.

### 🏛️ Mimari Kararlar ve Gerekçeler
Clean Architecture (N-Tier): Proje; Core, Infrastructure ve API katmanlarına bölünmüştür. Bu sayede veritabanı veya dış servis bağımlılıkları değişse bile çekirdek iş mantığı (Business Logic) korunur.

Repository Pattern: Veri erişim operasyonları soyutlanarak merkezi bir yapıya alınmış, böylece kod tekrarı önlenmiş ve test edilebilir bir yapı kurulmuştur.

Global Exception Handling: Özel bir Middleware aracılığıyla uygulama genelindeki tüm hatalar yakalanır ve kullanıcıya standart, güvenli bir JSON formatında sunulur.

Structured Logging: Serilog entegrasyonu ile her isteğin metodu, yolu, yanıt süresi ve hata detayları hem konsola hem de günlük dosyalara kaydedilir.

Rate Limiting: API güvenliği için Fixed Window algoritması kullanılarak IP bazlı hız sınırlandırması uygulanmıştır (1 dk / 10 istek). Sınır aşıldığında sistem otomatik olarak 429 Too Many Requests yanıtı döner.

### ⚠️ Eksik Bırakılan veya Geliştirilmesi Gereken Kısımlar
Unit Tests: Yaşadığım ailevi kayıp (vefat) nedeniyle Repository ve Controller katmanları için planlanan Unit Test (xUnit/Moq) çalışmaları tamamlanamamıştır.
