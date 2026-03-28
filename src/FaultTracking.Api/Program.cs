using Microsoft.EntityFrameworkCore;
using Core.Interfaces;
using Core.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// --- 1. VERİTABANI BAĞLANTISI ---
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- 2. REPOSITORY VE SERVICE KAYITLARI ---
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// --- 3. DİĞER SERVİSLER ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Uygulama ayağa kalkarken bir scope oluşturup SeedData'yı çalıştırıyoruz
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // SeedData.cs içindeki Initialize metodunu çağırıyoruz
        SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        // Eğer veritabanı o an hazır değilse (Docker'da bazen SQL geç açılır) hata logla
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Veritabanı beslenirken (seeding) bir hata oluştu.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || true) // Docker'da swagger görebilmek için || true ekledik
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();