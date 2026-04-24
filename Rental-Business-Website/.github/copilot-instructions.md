# Rental Business System - Project Information

## Project Overview
A Windows Forms C# application for managing rental equipment business with MySQL database integration via XAMPP.

## Technology Stack
- **Language**: C# (.NET 6.0)
- **UI Framework**: Windows Forms
- **Database**: MySQL (via XAMPP)
- **ORM/Data Access**: ADO.NET with MySql.Data package
- **Target Platform**: Windows Desktop

## Project Structure
```
Rental-Business-Website/
├── Models/           # Data models (RentalItem, Admin, Rental, Payment)
├── Data/             # Data access repositories
├── Forms/            # Windows Forms UI components
├── Utilities/        # Helper classes (PaymentProcessor)
├── Program.cs        # Application entry point
├── *.csproj          # Project configuration
└── database_setup.sql # Database initialization
```

## Key Features Implemented
1. Admin Login Form - Secure authentication
2. Admin Dashboard - Main interface with tabs for management
3. Rental Items Management - Add, edit, delete equipment
4. Rental Creation - Customer rentals with payment methods
5. Database Integration - Persistent storage in MySQL
6. Payment Methods - Multiple payment options support

## Database Setup
- Database: `rental_business_db`
- Default Admin: username=admin, password=admin123
- 10 pre-configured rental item categories
- Run `database_setup.sql` in phpMyAdmin to initialize

## Build & Run
```powershell
dotnet build          # Build project
dotnet run           # Run application
```

## Dependencies
- MySql.Data (8.0.33) - MySQL connector
- System.Configuration.ConfigurationManager - Configuration management

## Recent Changes
- Created complete project scaffolding with C# WinForms
- Implemented admin login and dashboard
- Added rental items and rental management forms
- Integrated MySQL database with repositories
- Added payment method selection
- Created comprehensive documentation

## Notes for Development
- Database credentials are in DataConnection.cs (Server=localhost;Uid=root;Pwd=;)
- Forms use Designer pattern with programmatic UI construction
- Implement security improvements (bcrypt passwords) before production
- Consider adding more payment gateway integrations (Stripe, PayPal)
