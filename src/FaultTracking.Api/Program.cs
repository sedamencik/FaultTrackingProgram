using Microsoft.EntityFrameworkCore;
using Core.Interfaces;
using Core.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// --- 1. VERİTABANI BAĞLANTISI ---
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- 2. REPOSITORY VE SERVICE KAYITLARI ---
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
//builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// --- 3. DİĞER SERVİSLER ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    // 1. Genel Bilgiler ve Annotations (Attribute kullanımı için)
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "FaultTracking API", Version = "v1" });
    opt.EnableAnnotations(); // Annotations desteğini burada aktif ediyoruz

    // 2. JWT Güvenlik Tanımı (Bearer otomatik ekleyen versiyon)
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Sadece JWT Token'ınızı yapıştırın. 'Bearer' kelimesini sistem otomatik ekleyecektir."
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
{
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer" // Bu ID, AddSecurityDefinition içinde verdiğin isimle (Bearer) aynı olmalı
            }
        },
        Array.Empty<string>()
    }
});

    // 3. XML Yorumları Dosya Yolu
    try 
    {
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        
        if (File.Exists(xmlPath))
        {
            opt.IncludeXmlComments(xmlPath);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Swagger XML yüklenirken hata oluştu: {ex.Message}");
    }
});

// 1. JWT Ayarlarını Oku ve Kaydet
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();

builder.Services.AddAutoMapper(typeof(Program).Assembly);

var app = builder.Build();

// Middleware Sıralaması 
app.UseAuthentication(); 
app.UseAuthorization();  

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