using Core.Entities;
using Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Data;

public static class SeedData
{
    /// <summary>
    /// Hedef seed'i tanımlayan kullanıcı adı — HasData veya eski denemelerden kalan DB'yi ayırt etmek için.
    /// </summary>
    private const string MarkerUserName = "admin_caner";

    public static void Initialize(IServiceProvider serviceProvider)
    {
        var env = serviceProvider.GetRequiredService<IHostEnvironment>();
        var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(SeedData));

        using var context = new AppDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>());

        context.Database.EnsureCreated();

        if (context.Users.Any(u => u.UserName == MarkerUserName))
        {
            logger.LogInformation(
                "SeedData: '{Marker}' mevcut; seed atlanıyor.",
                MarkerUserName);
            return;
        }

        // Eski HasData (admin_saha vb.) veya kısmi seed: Users dolu ama bizim kayıtlarımız yok.
        if (context.Users.Any())
        {
            if (!env.IsDevelopment())
            {
                logger.LogWarning(
                    "SeedData: Veritabanında kullanıcı var ama hedef seed ('{Marker}') yok. " +
                    "Production'da otomatik silinmez — TestDb'yi elle silin veya SEED sıfırlayın.",
                    MarkerUserName);
                return;
            }

            logger.LogWarning(
                "SeedData: Eski/karma veri algılandı (Development). Veritabanı silinip yeniden oluşturuluyor.");
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            context.ChangeTracker.Clear();
        }

        // --- 1. KULLANICILAR (Admin ve User Rolleri) ---
        var admins = new List<User>
        {
            new User { UserName = "admin_caner", Email = "caner@sirket.com", PasswordHash = "hash123", Role = Role.Admin },
            new User { UserName = "it_yonetici", Email = "it@sirket.com", PasswordHash = "hash123", Role = Role.Admin }
        };

        var users = new List<User>
        {
            new User { UserName = "zeynep_satis", Email = "zeynep@sirket.com", PasswordHash = "pass123", Role = Role.User },
            new User { UserName = "merve_ik", Email = "merve@sirket.com", PasswordHash = "pass123", Role = Role.User },
            new User { UserName = "ahmet_finans", Email = "ahmet@sirket.com", PasswordHash = "pass123", Role = Role.User },
            new User { UserName = "mehmet_depo", Email = "mehmet@sirket.com", PasswordHash = "pass123", Role = Role.User }
        };

        context.Users.AddRange(admins);
        context.Users.AddRange(users);
        context.SaveChanges();

        // --- 2. ARIZA BİLDİRİMLERİ (Çeşitli Senaryolar) ---
        var reports = new List<FaultReport>
        {
            new FaultReport
            {
                Title = "Ana Sunucu Erişilemiyor",
                Description = "ERP sistemine giriş yapılamıyor, tüm departmanlar durdu.",
                Location = "İstanbul / Maslak / Veri Merkezi",
                Priority = Priority.High, Status = FaultStatus.Inceleniyor,
                UserId = users[2].Id, CreatedAt = DateTime.UtcNow.AddHours(-1)
            },
            new FaultReport
            {
                Title = "Su Baskını - Sistem Odası",
                Description = "Üst kattan su sızıyor, cihazlar tehlikede.",
                Location = "Ankara / Çankaya / Merkez Bina",
                Priority = Priority.High, Status = FaultStatus.Inceleniyor,
                UserId = users[0].Id, CreatedAt = DateTime.UtcNow.AddHours(-3)
            },
            new FaultReport
            {
                Title = "Yazıcı Kağıt Sıkışması",
                Description = "2. kat koridor yazıcısı kullanılamıyor.",
                Location = "İzmir / Bornova / Bölge Ofisi",
                Priority = Priority.Medium, Status = FaultStatus.YeniKayit,
                UserId = users[1].Id, CreatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new FaultReport
            {
                Title = "VPN Hız Sorunu",
                Description = "Öğleden sonra bağlantı çok yavaşlıyor.",
                Location = "Uzaktan Çalışma / Ev",
                Priority = Priority.Medium, Status = FaultStatus.Inceleniyor,
                UserId = users[2].Id, CreatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new FaultReport
            {
                Title = "Klavye Tuş Değişimi",
                Description = "Enter tuşu basmıyor, yeni klavye talebi.",
                Location = "İstanbul / Maslak / A Blok",
                Priority = Priority.Low, Status = FaultStatus.Tamamlandi,
                UserId = users[3].Id, CreatedAt = DateTime.UtcNow.AddDays(-5),
                UpdatedAt = DateTime.UtcNow.AddDays(-4)
            },
            new FaultReport
            {
                Title = "Mail İmzası Güncelleme",
                Description = "Yeni unvanım imzama eklenmeli.",
                Location = "Dijital / Mail Sistemi",
                Priority = Priority.Low, Status = FaultStatus.Tamamlandi,
                UserId = users[0].Id, CreatedAt = DateTime.UtcNow.AddDays(-10),
                UpdatedAt = DateTime.UtcNow.AddDays(-9)
            },
            new FaultReport
            {
                Title = "Klima Kumandası Kayıp",
                Description = "Toplantı odasındaki kumanda yerinde yok.",
                Location = "Bursa / Nilüfer / Fabrika",
                Priority = Priority.Low, Status = FaultStatus.Iptal,
                UserId = users[1].Id, CreatedAt = DateTime.UtcNow.AddDays(-3)
            },
            new FaultReport
            {
                Title = "Excel Formül Hatası",
                Description = "Bütçe tablosunda toplamlar yanlış çıkıyor (Destek ricası).",
                Location = "İstanbul / Maslak / Finans Birimi",
                Priority = Priority.Medium, Status = FaultStatus.YeniKayit,
                UserId = users[2].Id, CreatedAt = DateTime.UtcNow
            }
        };

        context.FaultReports.AddRange(reports);
        context.SaveChanges();

        logger.LogInformation(
            "SeedData tamamlandı: {UserCount} kullanıcı, {ReportCount} bildirim.",
            context.Users.Count(),
            context.FaultReports.Count());
    }
}
