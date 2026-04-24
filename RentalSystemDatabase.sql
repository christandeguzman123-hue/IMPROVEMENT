-- Rental Business System Database Schema
-- Created for SQLite/SQL Server

-- Users Table (Admin & Customers)
CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(100) NOT NULL UNIQUE,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    FullName NVARCHAR(150) NOT NULL,
    PhoneNumber NVARCHAR(20),
    PasswordHash NVARCHAR(255) NOT NULL,
    UserType NVARCHAR(20) NOT NULL CHECK (UserType IN ('Admin', 'Customer')), -- Admin or Customer
    CreatedDate DATETIME DEFAULT GETDATE(),
    LastLogin DATETIME,
    IsActive BIT DEFAULT 1
);

-- Equipment Categories Table
CREATE TABLE EquipmentCategories (
    CategoryId INT PRIMARY KEY IDENTITY(1,1),
    CategoryName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    CreatedDate DATETIME DEFAULT GETDATE()
);

-- Equipment Table
CREATE TABLE Equipment (
    EquipmentId INT PRIMARY KEY IDENTITY(1,1),
    EquipmentName NVARCHAR(150) NOT NULL,
    CategoryId INT NOT NULL,
    Description NVARCHAR(500),
    DailyRentalPrice DECIMAL(10, 2) NOT NULL,
    Status NVARCHAR(50) NOT NULL CHECK (Status IN ('Available', 'Rented', 'Maintenance')),
    QuantityAvailable INT NOT NULL DEFAULT 1,
    QuantityTotal INT NOT NULL,
    ImageUrl NVARCHAR(255),
    LastUpdated DATETIME DEFAULT GETDATE(),
    CreatedDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (CategoryId) REFERENCES EquipmentCategories(CategoryId)
);

-- Customers Table (Extended User Info)
CREATE TABLE Customers (
    CustomerId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL UNIQUE,
    Address NVARCHAR(255),
    City NVARCHAR(100),
    State NVARCHAR(100),
    ZipCode NVARCHAR(20),
    TotalRentals INT DEFAULT 0,
    Status NVARCHAR(50) NOT NULL CHECK (Status IN ('Active', 'Inactive', 'VIP')) DEFAULT 'Active',
    CreatedDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

-- Payment Methods Table
CREATE TABLE PaymentMethods (
    PaymentMethodId INT PRIMARY KEY IDENTITY(1,1),
    CustomerId INT NOT NULL,
    PaymentType NVARCHAR(50) NOT NULL CHECK (PaymentType IN ('Credit Card', 'Debit Card', 'GCash', 'Apple Pay', 'Digital Wallet')),
    CardNumber NVARCHAR(20),
    CardholderName NVARCHAR(150),
    ExpiryDate NVARCHAR(10),
    CVV NVARCHAR(10),
    IsDefault BIT DEFAULT 0,
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (CustomerId) REFERENCES Customers(CustomerId)
);

-- Rentals Table
CREATE TABLE Rentals (
    RentalId INT PRIMARY KEY IDENTITY(1,1),
    CustomerId INT NOT NULL,
    EquipmentId INT NOT NULL,
    RentalDate DATE NOT NULL,
    ReturnDate DATE NOT NULL,
    DueDate DATE NOT NULL,
    RentalPrice DECIMAL(10, 2) NOT NULL,
    InsuranceFee DECIMAL(10, 2) DEFAULT 0,
    ProcessingFee DECIMAL(10, 2) DEFAULT 0,
    TotalPrice DECIMAL(10, 2) NOT NULL,
    Status NVARCHAR(50) NOT NULL CHECK (Status IN ('Pending', 'Approved', 'Ongoing', 'Completed', 'Overdue', 'Cancelled')) DEFAULT 'Pending',
    IsOverdue BIT DEFAULT 0,
    CreatedDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (CustomerId) REFERENCES Customers(CustomerId),
    FOREIGN KEY (EquipmentId) REFERENCES Equipment(EquipmentId)
);

-- Payments Table
CREATE TABLE Payments (
    PaymentId INT PRIMARY KEY IDENTITY(1,1),
    RentalId INT NOT NULL,
    CustomerId INT NOT NULL,
    PaymentMethodId INT,
    Amount DECIMAL(10, 2) NOT NULL,
    PaymentDate DATETIME DEFAULT GETDATE(),
    Status NVARCHAR(50) NOT NULL CHECK (Status IN ('Pending', 'Completed', 'Failed', 'Refunded')) DEFAULT 'Pending',
    TransactionNumber NVARCHAR(100),
    Notes NVARCHAR(500),
    FOREIGN KEY (RentalId) REFERENCES Rentals(RentalId),
    FOREIGN KEY (CustomerId) REFERENCES Customers(CustomerId),
    FOREIGN KEY (PaymentMethodId) REFERENCES PaymentMethods(PaymentMethodId)
);

-- Notifications Table
CREATE TABLE Notifications (
    NotificationId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Message NVARCHAR(1000) NOT NULL,
    NotificationType NVARCHAR(50),
    IsRead BIT DEFAULT 0,
    CreatedDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

-- Maintenance Log Table
CREATE TABLE MaintenanceLogs (
    MaintenanceId INT PRIMARY KEY IDENTITY(1,1),
    EquipmentId INT NOT NULL,
    MaintenanceDate DATE NOT NULL,
    Description NVARCHAR(500) NOT NULL,
    Status NVARCHAR(50) NOT NULL CHECK (Status IN ('Scheduled', 'In Progress', 'Completed')) DEFAULT 'Scheduled',
    CompletedDate DATE,
    Notes NVARCHAR(1000),
    CreatedDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (EquipmentId) REFERENCES Equipment(EquipmentId)
);

-- Audit Log Table
CREATE TABLE AuditLogs (
    AuditId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT,
    Action NVARCHAR(100) NOT NULL,
    TableName NVARCHAR(100),
    RecordId INT,
    OldValue NVARCHAR(MAX),
    NewValue NVARCHAR(MAX),
    Timestamp DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

-- Create Indexes for Performance
CREATE INDEX idx_Equipment_CategoryId ON Equipment(CategoryId);
CREATE INDEX idx_Equipment_Status ON Equipment(Status);
CREATE INDEX idx_Customers_UserId ON Customers(UserId);
CREATE INDEX idx_PaymentMethods_CustomerId ON PaymentMethods(CustomerId);
CREATE INDEX idx_Rentals_CustomerId ON Rentals(CustomerId);
CREATE INDEX idx_Rentals_EquipmentId ON Rentals(EquipmentId);
CREATE INDEX idx_Rentals_Status ON Rentals(Status);
CREATE INDEX idx_Rentals_DueDate ON Rentals(DueDate);
CREATE INDEX idx_Payments_RentalId ON Payments(RentalId);
CREATE INDEX idx_Payments_CustomerId ON Payments(CustomerId);
CREATE INDEX idx_MaintenanceLogs_EquipmentId ON MaintenanceLogs(EquipmentId);
