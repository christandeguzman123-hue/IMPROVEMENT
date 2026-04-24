# Rental Business System - Database Setup Guide

## Overview
This document provides instructions for setting up and configuring the Rental Business System database using SQL Server and integrating it with the C# WinForms application.

## Database Architecture

### Tables Overview
1. **Users** - System users (Admin and Customers)
2. **EquipmentCategories** - Equipment categories
3. **Equipment** - Equipment inventory
4. **Customers** - Customer information
5. **PaymentMethods** - Payment method details
6. **Rentals** - Rental transactions
7. **Payments** - Payment records
8. **Notifications** - System notifications
9. **MaintenanceLogs** - Equipment maintenance history
10. **AuditLogs** - Audit trail for data changes

## Setup Instructions

### 1. Create the Database

**Using SQL Server Management Studio (SSMS):**
1. Open SSMS
2. Connect to your SQL Server instance
3. Create a new database:
   ```sql
   CREATE DATABASE RentalBusinessSystem;
   ```

### 2. Run Database Schema Script

1. Open the `RentalSystemDatabase.sql` file in SSMS
2. Select the new `RentalBusinessSystem` database from the dropdown
3. Execute all queries to create the tables

### 3. Insert Sample Data (Optional)

1. Open the `SampleData.sql` file in SSMS
2. Select the `RentalBusinessSystem` database
3. Execute all queries to populate sample data

## Connection String Configuration

### Update Your Application

In your Program.cs or Application startup:

```csharp
// Define your connection string
string connectionString = "Server=YOUR_SERVER_NAME;Database=RentalBusinessSystem;User Id=YOUR_USERNAME;Password=YOUR_PASSWORD;";

// Create database connection
DatabaseConnection db = new DatabaseConnection(connectionString);

// Test connection
if (db.TestConnection())
{
    MessageBox.Show("Database connected successfully!");
}
else
{
    MessageBox.Show("Failed to connect to database");
}
```

### Connection String Options

**For Local SQL Server:**
```
Server=(local);Database=RentalBusinessSystem;Integrated Security=true;
```

**For Named Instance:**
```
Server=COMPUTER_NAME\INSTANCE_NAME;Database=RentalBusinessSystem;User Id=sa;Password=YourPassword;
```

**For Azure SQL:**
```
Server=your_server.database.windows.net;Database=RentalBusinessSystem;User Id=username;Password=password;Encrypt=true;TrustServerCertificate=false;Connection Timeout=30;
```

## Using Repository Classes

### 1. Equipment Operations

```csharp
// Initialize database connection
DatabaseConnection db = new DatabaseConnection(connectionString);
EquipmentRepository equipmentRepo = new EquipmentRepository(db);

// Get all equipment
List<Equipment> allEquipment = equipmentRepo.GetAllEquipment();

// Get equipment by ID
Equipment equipment = equipmentRepo.GetEquipmentById(1);

// Get available equipment
List<Equipment> available = equipmentRepo.GetEquipmentByStatus("Available");

// Add new equipment
Equipment newEquipment = new Equipment
{
    EquipmentName = "Drone",
    CategoryId = 1,
    DailyRentalPrice = 1700,
    Status = "Available",
    QuantityAvailable = 5,
    QuantityTotal = 5
};
int equipmentId = equipmentRepo.AddEquipment(newEquipment);

// Update equipment
equipment.Status = "Rented";
equipmentRepo.UpdateEquipment(equipment);

// Delete equipment
equipmentRepo.DeleteEquipment(1);
```

### 2. Rental Operations

```csharp
RentalRepository rentalRepo = new RentalRepository(db);

// Get all rentals
List<Rental> allRentals = rentalRepo.GetAllRentals();

// Get customer rentals
List<Rental> customerRentals = rentalRepo.GetRentalsByCustomer(customerId);

// Get overdue rentals
List<Rental> overdueRentals = rentalRepo.GetOverdueRentals();

// Create new rental
Rental newRental = new Rental
{
    CustomerId = 1,
    EquipmentId = 2,
    RentalDate = DateTime.Now,
    ReturnDate = DateTime.Now.AddDays(3),
    DueDate = DateTime.Now.AddDays(3),
    RentalPrice = 1000,
    InsuranceFee = 150,
    ProcessingFee = 100,
    TotalPrice = 1250,
    Status = "Pending"
};
int rentalId = rentalRepo.AddRental(newRental);

// Update rental status
newRental.Status = "Approved";
rentalRepo.UpdateRental(newRental);
```

## Data Model Relationships

```
Users (1) ── (1) Customers ── (M) Rentals ── (1) Equipment
   │                  │
   │                  └──── (M) PaymentMethods
   │                  │
   │                  └──── (M) Payments
   │
   └──── (M) Notifications
```

## User Types

- **Admin Users**: Full access to all system features
  - Default admin: `admin@rentalservice.com`
  
- **Customer Users**: Limited access to their own rentals and equipment browsing

## Key Features

### Authentication
- Username/Password-based authentication
- User types: Admin and Customer
- Last login tracking

### Equipment Management
- Equipment inventory tracking
- Status management (Available, Rented, Maintenance)
- Category-based organization
- Rental price tracking

### Rental Management
- Rental scheduling
- Payment tracking
- Overdue detection
- Insurance and processing fees

### Payment Processing
- Multiple payment methods support
- Payment status tracking
- Transaction logging

### Audit & Logging
- Complete audit trail
- Equipment maintenance logging
- System notifications

## Best Practices

1. **Always use parameterized queries** to prevent SQL injection
2. **Test database connection** on application startup
3. **Use try-catch blocks** for database operations
4. **Implement proper error logging**
5. **Use transactions** for multi-step operations
6. **Regularly backup the database**

## Troubleshooting

### Connection Issues
- Verify SQL Server is running
- Check connection string syntax
- Ensure firewall allows SQL Server port (default: 1433)
- Verify user credentials are correct

### Schema Issues
- Run the schema script again
- Check for duplicate table names
- Verify SQL Server version compatibility

### Performance Issues
- Review indexes created in schema
- Monitor query execution plans
- Consider adding additional indexes for frequently queried columns

## Security Considerations

1. **Password Storage**: Always hash passwords before storing (use BCrypt or similar)
2. **SQL Injection Prevention**: Always use parameterized queries
3. **Data Encryption**: Consider encrypting sensitive data at rest and in transit
4. **Access Control**: Implement role-based access control (RBAC)
5. **Audit Logging**: Log all critical operations

## Future Enhancements

- [ ] Entity Framework Core integration
- [ ] Database views for complex queries
- [ ] Stored procedures for performance optimization
- [ ] Data archival strategy
- [ ] Backup and recovery procedures
- [ ] Multi-tenancy support

---

**Last Updated**: April 17, 2026
**Version**: 1.0
