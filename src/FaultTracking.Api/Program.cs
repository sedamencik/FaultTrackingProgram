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

var builder = WebApplication.CreateBuilder(args);

// --- 1. VERİTABANI BAĞLANTISI ---
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- 2. REPOSITORY VE SERVICE KAYITLARI ---
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// --- 3. DİĞER SERVİSLER ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "FaultTracking API", Version = "v1" });

    // Buradaki kritik nokta: SecuritySchemeType.Http ve Scheme = "bearer"
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http, // ApiKey yerine Http seçtik
        Scheme = "bearer",             // Küçük harfle bearer
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
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
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