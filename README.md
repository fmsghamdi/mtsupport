# Mabat Support Ticket System

A comprehensive support ticket system module integrated into the Mabat hotel booking application.

## 🏨 About Mabat

Mabat is a hotel booking application that handles:
- **Hourly bookings**: 4 hours and 6 hours
- **Full Day bookings**: 8:00 PM to 12:00 PM (next day)

## 🛠️ Technology Stack

- **Backend**: .NET 8 (C#)
- **Frontend**: ASP.NET Core Razor Pages
- **Styling**: Bootstrap 5 with Bootstrap Icons
- **Database**: Entity Framework Core
  - Local Development: SQLite
  - Production: PostgreSQL

## 📋 Features

- ✅ Create support tickets with categories specific to hotel booking issues
- ✅ Priority levels (Low, Medium, High)
- ✅ Status tracking (Open, In Progress, Resolved, Closed)
- ✅ Ticket filtering and search functionality
- ✅ Conversation/response system
- ✅ Booking reference tracking
- ✅ Responsive design with Bootstrap 5

## 📁 Project Structure

```
mtsupport/
├── Data/
│   └── MabatSupportDbContext.cs       # Database context
├── Models/
│   ├── Ticket.cs                       # Main ticket entity
│   ├── TicketCategory.cs               # Category entity
│   ├── TicketResponse.cs               # Response/conversation entity
│   ├── TicketStatus.cs                 # Status enum
│   └── TicketPriority.cs               # Priority enum
├── Pages/
│   ├── Tickets/
│   │   ├── Create.cshtml/.cs           # Create ticket page
│   │   ├── Index.cshtml/.cs            # Ticket list/dashboard
│   │   └── Details.cshtml/.cs          # Ticket details & conversation
│   └── Shared/
│       └── _Layout.cshtml              # Main layout with navigation
├── appsettings.json                    # Configuration & connection strings
└── Program.cs                          # Application startup & DB config
```

## 🗄️ Database Configuration

### Switching Between SQLite and PostgreSQL

The application is configured to easily switch between database providers:

**Using SQLite (Default for Local Development)**
```json
"DatabaseProvider": "Sqlite"
```

**Using PostgreSQL (Production)**
```json
"DatabaseProvider": "PostgreSQL"
```

Edit `appsettings.json` to change the provider.

### Connection Strings

**SQLite** (Local Development):
```json
"SqliteConnection": "Data Source=mabat_support.db"
```

**PostgreSQL** (Production):
```json
"PostgresConnection": "Host=localhost;Port=5432;Database=mabat_support_db;Username=postgres;Password=your_password_here"
```

## 🚀 Getting Started

### Prerequisites

- .NET 8 SDK
- Visual Studio Code (or any IDE)
- For production: PostgreSQL server

### Installation Steps

1. **Clone or navigate to the project directory**
   ```bash
   cd c:\Users\fmghamdi.UQU\OneDrive\Documents\mtsupport
   ```

2. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

3. **Install EF Core Tools** (if not already installed)
   ```bash
   dotnet tool install --global dotnet-ef --version 8.0.0
   ```
   
   If already installed, update it:
   ```bash
   dotnet tool update --global dotnet-ef --version 8.0.0
   ```

4. **Create Database Migration**
   ```bash
   dotnet ef migrations add InitialCreate
   ```

5. **Apply Migration to Database**
   ```bash
   dotnet ef database update
   ```

6. **Run the Application**
   ```bash
   dotnet run
   ```

7. **Access the Application**
   - Open your browser at: `https://localhost:5001` or `http://localhost:5000`
   - Navigate to "Support Tickets" in the menu

## 📊 Pre-seeded Categories

The system comes with 8 pre-configured ticket categories:

1. **Check-in Issue** - Problems with check-in process or room access
2. **Extension Request (4h to 6h)** - Request to extend hourly booking
3. **Overnight Booking Issue** - Issues with full day bookings
4. **Hourly Booking (4h) Issue** - Problems with 4-hour bookings
5. **Hourly Booking (6h) Issue** - Problems with 6-hour bookings
6. **Payment & Billing** - Payment issues, refunds, or billing questions
7. **Cancellation & Refund** - Booking cancellation and refund requests
8. **General Inquiry** - General questions about the booking system

## 🎨 User Interface

### Ticket Creation
- Form-based ticket submission
- Category selection
- Priority setting
- Optional booking reference
- User information

### Ticket Dashboard
- List all tickets with status badges
- Filter by status and priority
- Search by title or booking reference
- Color-coded priority indicators
- Quick access to ticket details

### Ticket Details
- Full ticket information
- Conversation history
- Add responses
- Status and priority tracking
- Timestamp for all activities

## 🔄 Migrations & Database Updates

### Create a New Migration
```bash
dotnet ef migrations add MigrationName
```

### Apply Migrations
```bash
dotnet ef database update
```

### Remove Last Migration (if not applied)
```bash
dotnet ef migrations remove
```

### For Production (PostgreSQL)
1. Update `appsettings.json` to use PostgreSQL
2. Set `"DatabaseProvider": "PostgreSQL"`
3. Update the PostgreSQL connection string
4. Run migrations:
   ```bash
   dotnet ef database update
   ```

## 📝 Notes for Production Deployment

1. **Update Connection String**: Change PostgreSQL credentials in `appsettings.json`
2. **Set Database Provider**: Change `DatabaseProvider` to `PostgreSQL`
3. **Run Migrations**: Apply EF Core migrations to create database schema
4. **Security**: 
   - Use environment variables for sensitive data
   - Implement authentication (currently using demo user IDs)
   - Add authorization for staff responses
5. **Performance**: 
   - Enable connection pooling (already configured)
   - Add caching for categories
   - Implement pagination for large ticket lists

## 🛡️ Security Considerations

- Current implementation uses demo user IDs for testing
- In production, integrate with your authentication system
- Implement role-based authorization for staff features
- Validate and sanitize all user inputs
- Use HTTPS in production

## 🤝 Integration with Mabat System

To integrate with the main Mabat booking application:

1. Link `BookingReferenceId` to actual booking records
2. Add foreign key relationship to Booking table
3. Pull user information from authentication system
4. Add notification system for ticket updates
5. Create staff dashboard for ticket management

## 📞 Support

For issues or questions about this module, please create a support ticket using the system itself!

---

**Developed for Mabat Hotel Booking System**  
© 2025 - Support Ticket Module
