using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using MabatSupportSystem.Data;
using MabatSupportSystem.Models;
using MabatSupportSystem;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// =========================================================
// 1. إعداد نظام الترجمة
// =========================================================
builder.Services.AddLocalization();

// Add services to the container.
builder.Services.AddRazorPages()
    .AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization(options => {
        options.DataAnnotationLocalizerProvider = (type, factory) =>
            factory.Create(typeof(SharedResource));
    });

// Configure Request Localization Options
var supportedCultures = new[]
{
    new CultureInfo("en-US"),
    new CultureInfo("ar-SA")
};

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var arCulture = new RequestCulture("ar-SA");
    // إجبار النظام على استخدام التقويم الميلادي (Gregorian) حتى مع اللغة العربية
    // لتجنب مشاكل تواريخ الهجري في قواعد البيانات
    arCulture.Culture.DateTimeFormat.Calendar = new GregorianCalendar();
    arCulture.UICulture.DateTimeFormat.Calendar = new GregorianCalendar();

    options.DefaultRequestCulture = arCulture;
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
    
    // Use Cookie as primary provider, then Query String
    options.RequestCultureProviders = new List<IRequestCultureProvider>
    {
        new CookieRequestCultureProvider(),
        new QueryStringRequestCultureProvider()
    };
});

// Configure Database Provider
var databaseProvider = builder.Configuration.GetValue<string>("DatabaseProvider") ?? "Sqlite";

if (databaseProvider.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase))
{
    var connectionString = builder.Configuration.GetConnectionString("PostgresConnection") 
        ?? throw new InvalidOperationException("PostgreSQL connection string not found.");
    
    builder.Services.AddDbContext<MabatSupportDbContext>(options =>
        options.UseNpgsql(connectionString, 
            npgsqlOptions => npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorCodesToAdd: null)));
    
    Console.WriteLine("Using PostgreSQL Database Provider");
}
else
{
    var connectionString = builder.Configuration.GetConnectionString("SqliteConnection") 
        ?? "Data Source=mabat_support.db";
    
    builder.Services.AddDbContext<MabatSupportDbContext>(options =>
        options.UseSqlite(connectionString));
    
    Console.WriteLine("Using SQLite Database Provider");
}

// Configure Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    
    // User settings
    options.User.RequireUniqueEmail = true;
    
    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddEntityFrameworkStores<MabatSupportDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

var app = builder.Build();

// Auto-create database at startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider; 
    var db = services.GetRequiredService<MabatSupportDbContext>();
    
    try
    {
        db.Database.Migrate();
        Console.WriteLine("✅ Database created successfully!");

        await DbInitializer.Initialize(services);
        Console.WriteLine("✅ Admin account seeded successfully!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// 2. ترتيب الـ Middleware الصحيح (مهم جداً للصور والترجمة)
app.UseStaticFiles(); // الملفات الثابتة أولاً

app.UseRequestLocalization(); // ثم الترجمة

app.UseRouting(); // ثم التوجيه

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
