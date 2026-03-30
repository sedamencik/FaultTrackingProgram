# FaultTrackingProgram

# 🛠️ Fault Tracking System (Arıza Takip Sistemi)
Bu proje, kurumsal ortamlarda teknik arıza bildirimlerini yönetmek, takip etmek ve durumlarını güncellemek için geliştirilmiş ASP.NET Core Web API tabanlı bir sistemdir.

🚀 Projeyi Çalıştırma Adımları
Projeyi çalıştırmak için bilgisayarınızda Docker ve Docker Desktop kurulu olmalıdır.

Projeyi Klonlayın:

Bash
git clone https://github.com/sedamencik/FaultTrackingProgram.git
cd src/FaultTracking.Api
Sistemi Ayağa Kaldırın (Docker Compose):

### 🚀 Projeyi Başlatma (Temiz Kurulum)
Uygulamayı ve veritabanını tüm bağımlılıklarıyla birlikte, Docker cache'ini temizleyerek ayağa kaldırmak için:

Bash
docker compose down -v
docker compose up -d --build


Veritabanı Migration:
Konteynerlar ayağa kalktığında API, veritabanına otomatik olarak bağlanır. Eğer ilk kurulumda tablolar oluşmazsa API projesi içinde şu komutu çalıştırabilirsiniz:

Bash
dotnet ef database update
Erişim:

Swagger UI: http://localhost:5005/swagger

API Base URL: http://localhost:5005/swagger/index.html

🏗️ Kullanılan Teknolojiler ve Kütüphaneler
Framework: .NET 8.0 (Web API)

Database: Microsoft SQL Server

ORM: Entity Framework Core

Mapping: AutoMapper (Entity-DTO dönüşümleri için)

Logging: Serilog (Konsol ve dosya tabanlı yapılandırılmış loglama)

Containerization: Docker & Docker Compose

Documentation: Swagger / OpenApi

🔄 Durum Makinesi (State Machine) Akışı
Sistemdeki arıza bildirimleri kontrolsüz bir şekilde durum değiştiremez. Aşağıdaki mantıksal akış (State Machine) uygulanmıştır:

YeniKayit: Her bildirim bu statüde başlar.

Inceleniyor: Sadece YeniKayit olanlar bu duruma geçebilir.

Atandi/Calisiliyor: İnceleme sonrası teknik ekibe yönlendirme aşaması.

Tamamlandi: Sadece Calisiliyor durumundaki bir kayıt kapatılabilir.

Iptal/Asilsiz: Kayıt herhangi bir aşamada (Tamamlanmadıysa) iptal edilebilir.

Not: Geçersiz bir durum değişikliği isteğinde API 422 Unprocessable Entity hatası döner.

🏛️ Mimari Kararlar ve Gerekçeler
Clean Architecture (N-Tier): Proje; Core, Infrastructure ve API katmanlarına bölünmüştür. Bu sayede veritabanı teknolojisi değişse bile iş mantığı (Business Logic) etkilenmez.

Repository Pattern: Veri erişim kodları merkezi bir yerde toplanarak kod tekrarı önlenmiş ve test edilebilirlik artırılmıştır.

Generic Result Structure: Tüm API cevapları SuccessDataResult veya ErrorResult gibi standart bir yapıda döner. Bu, frontend tarafının hata yönetimini kolaylaştırır.

Custom Logging Middleware: Serilog ile entegre çalışan bir middleware yazılarak; her isteğin metodu, yolu, yanıt süresi ve hata detayları merkezi olarak kayıt altına alınmıştır.

⚠️ Eksik Bırakılan veya Geliştirilmesi Gereken Kısımlar
Unit Tests: Aile ferdimin vefatı nedeniyle Repository ve Controller katmanları için Unit Test'ler yazılamamıştır.








🛠️ Fault Tracking System (Arıza Takip Sistemi)
Bu proje, kurumsal ortamlarda teknik arıza bildirimlerini yönetmek, takip etmek ve durumlarını profesyonel bir iş akışıyla (State Machine) güncellemek için geliştirilmiş ASP.NET Core 8 Web API tabanlı bir sistemdir.

🚀 Projeyi Çalıştırma Adımları
Projeyi çalıştırmak için bilgisayarınızda Docker ve Docker Desktop kurulu olmalıdır.

1. Projeyi Klonlayın
Bash
git clone https://github.com/sedamencik/FaultTrackingProgram.git
cd FaultTrackingProgram
2. Sistemi Ayağa Kaldırın (Temiz Kurulum)
Aşağıdaki komut; SQL Server ve Web API konteynerlarını yapılandırır, bağımlılıkları yükler ve sistemi başlatır. -v parametresi veritabanını sıfırlayarak temiz bir başlangıç yapmanızı sağlar.

Bash
docker compose down -v
docker compose up -d --build
3. Veritabanı ve Erişim
Konteynerlar ayağa kalktığında API, SQL Server'a otomatik olarak bağlanır.

Swagger UI: http://localhost:5005/swagger

API Base URL: http://localhost:5005/api

🏗️ Kullanılan Teknolojiler ve Kütüphaneler
Framework: .NET 8.0 (Web API)

Database: Microsoft SQL Server

ORM: Entity Framework Core (Code First)

Mapping: AutoMapper (Entity-DTO dönüşümleri)

Logging: Serilog (Konsol ve dosya tabanlı structured logging)

Containerization: Docker & Docker Compose

Documentation: Swagger / OpenAPI

🔄 Durum Makinesi (State Machine) Akışı
Sistemdeki arıza bildirimleri kontrolsüz bir şekilde durum değiştiremez. Veri bütünlüğünü korumak adına aşağıdaki mantıksal akış uygulanmıştır:

YeniKayit: Her bildirim bu statüde başlar.

Inceleniyor: Sadece YeniKayit olan bildirimler bu aşamaya geçebilir.

Atandi / Calisiliyor: İnceleme sonrası teknik ekibe yönlendirme aşaması.

Tamamlandi: Sadece Calisiliyor durumundaki bir kayıt başarıyla kapatılabilir.

Iptal / Asilsiz: Kayıt, tamamlanmadığı sürece herhangi bir aşamada iptal edilebilir.

[!IMPORTANT]
Geçersiz bir durum değişikliği isteğinde (örn: Yeni kaydı direkt tamamlandıya çekmek) API 422 Unprocessable Entity hatası döner.

🏛️ Mimari Kararlar ve Gerekçeler
Clean Architecture (N-Tier): Proje; Core, Infrastructure ve API katmanlarına bölünmüştür. Bu sayede veritabanı veya dış servis bağımlılıkları değişse bile çekirdek iş mantığı (Business Logic) korunur.

Repository Pattern: Veri erişim operasyonları soyutlanarak merkezi bir yapıya alınmış, böylece kod tekrarı önlenmiş ve test edilebilir bir yapı kurulmuştur.

Global Exception Handling: Özel bir Middleware aracılığıyla uygulama genelindeki tüm hatalar yakalanır ve kullanıcıya standart, güvenli bir JSON formatında sunulur.

Structured Logging: Serilog entegrasyonu ile her isteğin metodu, yolu, yanıt süresi ve hata detayları hem konsola hem de günlük dosyalara kaydedilir.

⚠️ Eksik Bırakılan veya Geliştirilmesi Gereken Kısımlar
Unit Tests: Yaşadığım ailevi kayıp (vefat) nedeniyle Repository ve Controller katmanları için planlanan Unit Test (xUnit/Moq) çalışmaları tamamlanamamıştır.

Identity & Role Based Auth: Kullanıcı rollerine göre endpoint bazlı kısıtlamaların (Admin/User ayrımı) derinleştirilmesi planlanmaktadır.
