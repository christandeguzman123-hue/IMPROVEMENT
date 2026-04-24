# Rental Business System

A comprehensive Windows Forms application for managing rental equipment business operations. Built with C# and MySQL (XAMPP).

## Features

- **Admin Login**: Secure authentication for administrators
- **Admin Dashboard**: Centralized management interface with tabs for:
  - Rental Items Management (Add, Edit, Delete)
  - Rental Transactions View
  - Statistics and Reports
- **Rental Items Catalog**: 10 pre-configured equipment categories:
  - Camera
  - Projector
  - Speaker
  - Laptop
  - Microphone
  - Lighting Kit
  - Tripod
  - Drone
  - Tablet
  - VR Headset
- **Rental Management**: Create and manage rental transactions
- **Multiple Payment Methods**: Credit Card, Debit Card, Bank Transfer, Cash, Online Payment
- **MySQL Database**: Persistent data storage with XAMPP integration

## Prerequisites

Before running the application, ensure you have:

1. **.NET 6.0 SDK** or later installed
2. **XAMPP** installed with MySQL server
3. **Visual Studio Code** or **Visual Studio** for development

## Installation Steps

### 1. Setup XAMPP MySQL Database

1. Start XAMPP Control Panel
2. Start the Apache and MySQL modules
3. Click "Admin" next to MySQL to open phpMyAdmin (usually `http://localhost/phpmyadmin`)
4. Import the database schema:
   - Open phpMyAdmin
   - Click "Import" tab
   - Upload `database_setup.sql` file
   - Click "Go" to execute the script

Or manually run the SQL queries from `database_setup.sql`:
- Create database: `CREATE DATABASE rental_business_db;`
- Create all tables using the SQL file content

### 2. Clone/Download Project

```bash
cd c:\Main Events\Rental-Business-Website
```

### 3. Build the Project

Open Terminal/PowerShell in the project directory:

```powershell
dotnet build
```

This will restore NuGet packages and compile the project.

### 4. Run the Application

```powershell
dotnet run
```

Or directly run the compiled executable from `bin/Debug/net6.0-windows/`

## Default Login Credentials

- **Username**: admin
- **Password**: admin123

⚠️ **Important**: Change these credentials after first login in a production environment.

## Application Usage

### Admin Login
- Enter username and password on the login form
- Click "Login" to access the dashboard

### Dashboard Features

#### Rental Items Tab
- **View Items**: See all available rental equipment
- **Add Item**: Click "Add Item" to create new rental item
- **Edit Item**: Select an item and click "Edit Item" to modify details
- **Delete Item**: Select an item and click "Delete Item" to deactivate it

#### Rentals Tab
- **View All Rentals**: See all rental transactions
- **Rental Details**: Customer info, dates, amounts, payment status

#### Statistics Tab
- **Total Items**: Count of active rental items
- **Total Rentals**: Count of all rental transactions

### Creating a Rental
1. Select an item from the inventory
2. Click to create new rental
3. Enter customer information
4. Select rental dates
5. Choose payment method
6. Review total cost
7. Confirm rental

## Database Structure

### Tables

1. **admins**
   - ID, Username, Password Hash, Email, Created Date, Is Active

2. **rental_items**
   - ID, Name, Description, Daily Rate, Quantity Available, Category, Image Path, Is Active

3. **rentals**
   - ID, Item ID, Customer Info, Rental Dates, Quantity, Total Cost, Payment Method, Status

4. **payments**
   - ID, Rental ID, Amount, Payment Method, Transaction ID, Payment Status

## API Connection

The application connects to MySQL on:
- **Server**: localhost
- **Database**: rental_business_db
- **User**: root
- **Password**: (empty by default in XAMPP)
- **Port**: 3306

### Modify Connection String

To change database credentials, edit `Data/DatabaseConnection.cs`:

```csharp
private static readonly string ConnectionString = "Server=localhost;Database=rental_business_db;Uid=root;Pwd=;Port=3306;";
```

## Project Structure

```
Rental-Business-Website/
├── Models/                 # Data models
│   ├── RentalItem.cs
│   ├── Admin.cs
│   ├── Rental.cs
│   └── Payment.cs
├── Data/                   # Data access layer
│   ├── DatabaseConnection.cs
│   ├── AdminRepository.cs
│   ├── RentalItemRepository.cs
│   └── RentalRepository.cs
├── Forms/                  # Windows Forms UI
│   ├── AdminLoginForm.cs
│   ├── AdminDashboardForm.cs
│   ├── RentalItemForm.cs
│   └── RentalForm.cs
├── Utilities/              # Helper classes
│   └── PaymentProcessor.cs
├── Program.cs              # Application entry point
├── RentalBusinessSystem.csproj
├── database_setup.sql      # Database initialization script
└── README.md
```

## Development

### Adding New Features

1. **New Data Model**: Add class to `Models/` folder
2. **Database Operations**: Create repository in `Data/` folder
3. **UI Form**: Create Windows Form in `Forms/` folder
4. **Database Changes**: Update `database_setup.sql`

### Building for Release

```powershell
dotnet publish -c Release -o ./publish
```

This creates a release build in the `publish` folder ready for distribution.

## Troubleshooting

### Database Connection Error
- Ensure XAMPP MySQL server is running
- Check database credentials in `Data/DatabaseConnection.cs`
- Verify database exists: `rental_business_db`

### Build Errors
- Ensure .NET 6.0 SDK is installed: `dotnet --version`
- Clean build: `dotnet clean && dotnet build`

### Login Issues
- Check admin account exists in database
- Verify password hash in database (should be MD5 of "admin123")
- Run `database_setup.sql` to reset default admin

## Security Notes

- Passwords are hashed using MD5 (upgrade to bcrypt for production)
- Use HTTPS for network communications
- Implement role-based access control for production
- Never commit database credentials to version control

## License

This project is provided as-is for educational and business use.

## Support

For issues or questions, contact: admin@rentalbizsystem.com

---

**Version**: 1.0.0  
**Last Updated**: April 2026  
**Author**: Rental Business System Development Team
