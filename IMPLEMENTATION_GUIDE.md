# Complete Implementation Guide
## Identity, Localization & Bug Fixes for Mabat Support System

This guide provides all the code changes needed to implement:
1. Bug Fix: Data saving with validation
2. ASP.NET Core Identity (Login/Register/Logout)
3. Localization (Arabic & English with RTL support)

---

## STEP 1: Update Program.cs

Replace your entire `Program.cs` with this:

```csharp
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using MabatSupportSystem.Data;
using MabatSupportSystem.Models;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Configure Localization
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("en"),
        new CultureInfo("ar")
    };
    options.DefaultRequestCulture = new RequestCulture("en");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
    options.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider());
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
    var db = scope.ServiceProvider.GetRequiredService<MabatSupportDbContext>();
    try
    {
        db.Database.EnsureCreated();
        Console.WriteLine("✅ Database created and seeded successfully!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Database creation failed: {ex.Message}");
        throw;
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Use Localization
app.UseRequestLocalization();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
```

---

## STEP 2: Update Ticket Model

Update `Models/Ticket.cs` to add navigation property to ApplicationUser:

Add this property to the Ticket class:

```csharp
// Navigation property to ApplicationUser
public ApplicationUser? User { get; set; }
```

---

## STEP 3: Update Create.cshtml.cs (Bug Fix + Identity Integration)

Replace the entire `Pages/Tickets/Create.cshtml.cs` with:

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MabatSupportSystem.Data;
using MabatSupportSystem.Models;

namespace MabatSupportSystem.Pages.Tickets;

[Authorize]  // Require authentication
public class CreateModel : PageModel
{
    private readonly MabatSupportDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public CreateModel(MabatSupportDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [BindProperty]
    public Ticket Ticket { get; set; } = new();

    public SelectList Categories { get; set; } = null!;
    
    [TempData]
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        await LoadCategoriesAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Log validation errors for debugging
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            
            ErrorMessage = "Validation failed: " + string.Join(", ", errors);
            await LoadCategoriesAsync();
            return Page();
        }

        try
        {
            // Get current authenticated user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            // Set system fields
            Ticket.UserId = user.Id;
            Ticket.UserName = user.FullName ?? user.Email ?? "Unknown";
            Ticket.CreatedDate = DateTime.UtcNow;
            Ticket.Status = TicketStatus.Open;

            _context.Tickets.Add(Ticket);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Ticket #{Ticket.Id} created successfully!";
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to save ticket: {ex.Message}";
            await LoadCategoriesAsync();
            return Page();
        }
    }

    private async Task LoadCategoriesAsync()
    {
        var categories = await _context.TicketCategories
            .OrderBy(c => c.Name)
            .ToListAsync();
        
        Categories = new SelectList(categories, "Id", "Name");
    }
}
```

---

## STEP 4: Update Index.cshtml.cs (Filter by User)

Replace `Pages/Tickets/Index.cshtml.cs` with:

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MabatSupportSystem.Data;
using MabatSupportSystem.Models;

namespace MabatSupportSystem.Pages.Tickets;

[Authorize]
public class IndexModel : PageModel
{
    private readonly MabatSupportDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public IndexModel(MabatSupportDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public List<Ticket> Tickets { get; set; } = new();
    
    [BindProperty(SupportsGet = true)]
    public TicketStatus? StatusFilter { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public TicketPriority? PriorityFilter { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public string? SearchString { get; set; }

    public bool IsAdmin { get; set; }

    public async Task OnGetAsync(TicketStatus? statusFilter, TicketPriority? priorityFilter, string? searchString)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return;

        // Check if user is admin (you can customize this logic)
        IsAdmin = await _userManager.IsInRoleAsync(user, "Admin");

        var query = _context.Tickets
            .Include(t => t.Category)
            .AsQueryable();

        // Filter by user - only show own tickets unless admin
        if (!IsAdmin)
        {
            query = query.Where(t => t.UserId == user.Id);
        }

        // Apply filters
        if (statusFilter.HasValue)
        {
            query = query.Where(t => t.Status == statusFilter.Value);
        }

        if (priorityFilter.HasValue)
        {
            query = query.Where(t => t.Priority == priorityFilter.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            query = query.Where(t => 
                t.Title.Contains(searchString) || 
                (t.BookingReferenceId != null && t.BookingReferenceId.Contains(searchString)));
        }

        Tickets = await query
            .OrderByDescending(t => t.CreatedDate)
            .ToListAsync();
    }
}
```

---

## STEP 5: Update _Layout.cshtml (Add Login Partial + Language Switcher + RTL)

Replace `Pages/Shared/_Layout.cshtml` with this version that includes RTL support:

```html
@using Microsoft.AspNetCore.Mvc.Localization
@using System.Globalization
@inject IViewLocalizer Localizer

@{
    var currentCulture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
    var isArabic = currentCulture == "ar";
    var dir = isArabic ? "rtl" : "ltr";
    var lang = isArabic ? "ar" : "en";
}

<!DOCTYPE html>
<html lang="@lang" dir="@dir">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"] - Mabat Support System</title>
    
    @if (isArabic)
    {
        <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.rtl.min.css" />
    }
    else
    {
        <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" />
    }
    
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.0/font/bootstrap-icons.css">
    <link rel="stylesheet" href="~/css/site.css" asp-append-version="true" />
</head>
<body>
    <header>
        <nav class="navbar navbar-expand-lg navbar-dark bg-primary shadow-sm">
            <div class="container">
                <a class="navbar-brand fw-bold" asp-page="/Index">
                    <i class="bi bi-headset"></i> Mabat Support
                </a>
                <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
                    <span class="navbar-toggler-icon"></span>
                </button>
                <div class="collapse navbar-collapse" id="navbarNav">
                    <ul class="navbar-nav me-auto">
                        <li class="nav-item">
                            <a class="nav-link" asp-page="/Index">
                                <i class="bi bi-house"></i> Home
                            </a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" asp-page="/Tickets/Index">
                                <i class="bi bi-inbox"></i> Support Tickets
                            </a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" asp-page="/Tickets/Create">
                                <i class="bi bi-plus-circle"></i> Create Ticket
                            </a>
                        </li>
                    </ul>
                    
                    <!-- Language Switcher -->
                    <div class="d-flex align-items-center gap-2">
                        <div class="btn-group" role="group">
                            <a href="?culture=en&ui-culture=en" 
                               class="btn btn-sm @(currentCulture == "en" ? "btn-light" : "btn-outline-light")">
                                EN
                            </a>
                            <a href="?culture=ar&ui-culture=ar" 
                               class="btn btn-sm @(currentCulture == "ar" ? "btn-light" : "btn-outline-light")">
                                AR
                            </a>
                        </div>
                        
                        <!-- Login Partial -->
                        <partial name="_LoginPartial" />
                    </div>
                </div>
            </div>
        </nav>
    </header>
    
    <main role="main" class="pb-4">
        @RenderBody()
    </main>
    
    <footer class="border-top footer text-muted bg-light py-3 mt-5">
        <div class="container text-center">
            &copy; 2024 - Mabat Support System
        </div>
    </footer>

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
    <script src="~/js/site.js" asp-append-version="true"></script>
    @await RenderSectionAsync("Scripts", required: false)
</body>
</html>
```

---

## STEP 6: Create _LoginPartial.cshtml

Create a new file `Pages/Shared/_LoginPartial.cshtml`:

```html
@using Microsoft.AspNetCore.Identity
@using MabatSupportSystem.Models
@inject SignInManager<ApplicationUser> SignInManager
@inject UserManager<ApplicationUser> UserManager

@if (SignInManager.IsSignedIn(User))
{
    var user = await UserManager.GetUserAsync(User);
    <div class="dropdown">
        <button class="btn btn-outline-light dropdown-toggle" type="button" data-bs-toggle="dropdown">
            <i class="bi bi-person-circle"></i> @(user?.FullName ?? user?.Email ?? "User")
        </button>
        <ul class="dropdown-menu dropdown-menu-end">
            <li>
                <a class="dropdown-item" asp-area="Identity" asp-page="/Account/Manage/Index">
                    <i class="bi bi-gear"></i> Profile
                </a>
            </li>
            <li><hr class="dropdown-divider"></li>
            <li>
                <form class="form-inline" asp-area="Identity" asp-page="/Account/Logout" asp-route-returnUrl="@Url.Page("/Index", new { area = "" })">
                    <button type="submit" class="dropdown-item">
                        <i class="bi bi-box-arrow-right"></i> Logout
                    </button>
                </form>
            </li>
        </ul>
    </div>
}
else
{
    <div class="d-flex gap-2">
        <a class="btn btn-outline-light" asp-area="Identity" asp-page="/Account/Login">
            <i class="bi bi-box-arrow-in-right"></i> Login
        </a>
        <a class="btn btn-light" asp-area="Identity" asp-page="/Account/Register">
            <i class="bi bi-person-plus"></i> Register
        </a>
    </div>
}
```

---

## STEP 7: Delete Old Database and Rebuild

Run these commands:

```bash
# Stop any running instances
taskkill /F /IM dotnet.exe

# Delete old database
del mabat_support.db

# Build project
dotnet build MabatSupportSystem.csproj

# Run application (Identity tables will be created automatically)
dotnet run --project MabatSupportSystem.csproj
```

---

## STEP 8: Test the Application

1. Navigate to `http://localhost:5080`
2. Click "Register" to create a new account
3. Fill in: Email, Password, Confirm Password
4. Login with your credentials
5. Create a ticket - it will be saved with your UserId automatically
6. View your tickets - you'll only see YOUR tickets
7. Test language switcher (EN/AR) - notice RTL layout when Arabic is selected

---

## Summary of Changes

✅ **Bug Fix**: Added try-catch, validation error logging, and proper error handling
✅ **Identity**: Full authentication with Login/Register/Logout
✅ **User Filtering**: Users only see their own tickets (unless Admin role)
✅ **Localization**: EN/AR language switcher
✅ **RTL Support**: Automatic Bootstrap RTL stylesheet when Arabic is selected
✅ **Auto UserID Assignment**: Tickets automatically get the logged-in user's ID

---

## Admin Role (Optional)

To create an admin user, run this after first user registration:

```bash
dotnet user-secrets set "AdminEmail" "your

@admin.com"
```

Then add this code in Program.cs after database creation:

```csharp
// Create admin role and user
var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

if (!await roleManager.RoleExistsAsync("Admin"))
{
    await roleManager.CreateAsync(new IdentityRole("Admin"));
}

var adminEmail = "admin@mabat.com";
var adminUser = await userManager.FindByEmailAsync(adminEmail);
if (adminUser != null)
{
    await userManager.AddToRoleAsync(adminUser, "Admin");
}
```
