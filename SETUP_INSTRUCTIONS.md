# Setup Instructions for Mabat Support Ticket System

## Quick Start Guide

Follow these steps to get the application running:

### Step 1: Add dotnet-ef to PATH

The EF Core tools were installed but need to be added to your PATH:

```cmd
setx PATH "%PATH%;C:\Users\fmghamdi.UQU\.dotnet\tools"
```

**Important:** After running this command, close and reopen your terminal/command prompt for changes to take effect.

### Step 2: Create Database Migration

After reopening your terminal, navigate to the project directory and run:

```bash
cd c:\Users\fmghamdi.UQU\OneDrive\Documents\mtsupport
dotnet ef migrations add InitialCreate --project MabatSupportSystem.csproj
```

### Step 3: Apply Migration to Create Database

```bash
dotnet ef database update --project MabatSupportSystem.csproj
```

This will create a SQLite database file named `mabat_support.db` in the project directory with all the necessary tables and seeded data.

### Step 4: Run the Application

```bash
dotnet run --project MabatSupportSystem.csproj
```

### Step 5: Access the Application

Open your browser and navigate to:
- HTTPS: `https://localhost:5001`
- HTTP: `http://localhost:5000`

Click on "Support Tickets" in the navigation menu to start using the system.

---

## Alternative: Manual Database Creation (If EF Tools Don't Work)

If you continue to have issues with dotnet-ef tools, you can manually create the database:

### Option 1: Add Migration Code at Startup

Add this to `Program.cs` before `app.Run();`:

```csharp
// Auto-apply migrations at startup (for development only)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MabatSupportDbContext>();
    db.Database.Migrate();
}
```

Then just run: `dotnet run`

### Option 2: Use EnsureCreated (Quick but not recommended for production)

Replace the above code with:

```csharp
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MabatSupportDbContext>();
    db.Database.EnsureCreated();
}
```

---

## Switching to PostgreSQL

1. Install PostgreSQL server
2. Create database: `mabat_support_db`
3. Update `appsettings.json`:
   ```json
   "DatabaseProvider": "PostgreSQL",
   "PostgresConnection": "Host=localhost;Port=5432;Database=mabat_support_db;Username=postgres;Password=YourPassword"
   ```
4. Run migrations:
   ```bash
   dotnet ef database update --project MabatSupportSystem.csproj
   ```

---

## Testing the System

1. **Create a Ticket**: Click "Create Ticket" and fill out the form
2. **View Tickets**: Go to "Support Tickets" to see all tickets
3. **Filter/Search**: Use the filter options to find specific tickets
4. **Add Responses**: Click on a ticket to view details and add responses

---

## Troubleshooting

### Issue: "dotnet-ef not found"
**Solution**: Make sure you've added the tools directory to PATH and restarted your terminal.

### Issue: "Migration already exists"
**Solution**: Run `dotnet ef database update` to apply existing migrations.

### Issue: "Cannot connect to database"
**Solution**: Check your connection string in `appsettings.json`.

---

## Development Tips

- The system uses SQLite by default for easy local development
- All timestamps are stored in UTC
- The database is seeded with 8 ticket categories automatically
- For production, switch to PostgreSQL by changing the DatabaseProvider setting

---

**Ready to Start!** Follow the steps above and you'll be up and running in minutes.
