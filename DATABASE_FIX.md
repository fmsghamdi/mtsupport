# 🔧 DATABASE FIX - Execute These Commands NOW

## Step 1: Stop the Running Server
Press `Ctrl+C` in your terminal to stop the application.

## Step 2: Add dotnet-ef to PATH (One Time Only)
```cmd
setx PATH "%PATH%;C:\Users\fmghamdi.UQU\.dotnet\tools"
```
**Then close and reopen your terminal/PowerShell!**

## Step 3: Create Migration
Open a NEW terminal in your project directory and run:
```bash
cd c:\Users\fmghamdi.UQU\OneDrive\Documents\mtsupport
dotnet ef migrations add InitialCreate --project MabatSupportSystem.csproj
```

## Step 4: Apply Migration to Database
```bash
dotnet ef database update --project MabatSupportSystem.csproj
```

## Step 5: Run Application
```bash
dotnet run --project MabatSupportSystem.csproj
```

---

## If dotnet-ef Still Not Found

Use this alternative approach - add auto-migration at startup:

Open `Program.cs` and add this code BEFORE `app.Run();`:

```csharp
// Auto-apply migrations at startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MabatSupportDbContext>();
    db.Database.Migrate();
}
```

Then just run: `dotnet run --project MabatSupportSystem.csproj`
