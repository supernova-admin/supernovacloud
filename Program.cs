using CloudFileManager.Data;
using CloudFileManager.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Connection String'i appsettings.json'dan okuyoruz
// appsettings.json örneği:
// {
//   "ConnectionStrings": {
//     "DefaultConnection": "Data Source=cloudfilemanager.db"
//   }
// }
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// FileService, EF Core contextine ihtiyaç duyuyor; bu yüzden önce DbContext'i ekledik
builder.Services.AddScoped<FileService>();

// Razor Pages desteği
builder.Services.AddRazorPages();

// Web API controller'ları
builder.Services.AddControllers();

// CORS ayarları (Frontend farklı bir origin kullanabilir)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Veritabanı migrasyonlarını uygulayalım
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // Pending migration'lar varsa veritabanına uygular
    db.Database.Migrate();
}

// Static dosyaları (wwwroot) servise aç
app.UseStaticFiles();

// Routing
app.UseRouting();

// CORS politika uygulama
app.UseCors("AllowAll");

// API ve Razor Pages route ayarları
app.MapControllers();
app.MapRazorPages();

app.Run();
