-- Sample Data for Rental Business System

-- Insert Equipment Categories
INSERT INTO EquipmentCategories (CategoryName, Description) VALUES
('Video', 'Video recording and display equipment'),
('Audio', 'Audio recording and playback equipment'),
('Computing', 'Computing devices and accessories'),
('Display', 'Display and projection equipment'),
('Studio', 'Studio lighting and support equipment'),
('Mobile', 'Mobile devices and tablets'),
('Gaming', 'Gaming and VR equipment');

-- Insert Equipment
INSERT INTO Equipment (EquipmentName, CategoryId, Description, DailyRentalPrice, Status, QuantityAvailable, QuantityTotal, LastUpdated) VALUES
('Camera', 1, 'Professional camera for photography and video recording', 850.00, 'Available', 5, 10, GETDATE()),
('Projector', 3, 'High-quality projector for presentations', 1100.00, 'Rented', 3, 8, GETDATE()),
('Speaker', 2, 'Professional audio speaker', 550.00, 'Rented', 2, 5, GETDATE()),
('Laptop', 3, 'High-performance laptop for work and presentations', 1400.00, 'Maintenance', 0, 3, GETDATE()),
('Microphone', 2, 'Professional microphone for events', 450.00, 'Available', 8, 10, GETDATE()),
('Lighting Kit', 4, 'Professional lighting for photography and video', 1000.00, 'Available', 4, 6, GETDATE()),
('Tripod', 4, 'Sturdy tripod stand for cameras', 450.00, 'Available', 7, 12, GETDATE()),
('Drone', 1, 'Aerial photography drone', 1700.00, 'Rented', 1, 3, GETDATE()),
('Tablet', 5, 'Portable tablet for presentations', 400.00, 'Rented', 2, 4, GETDATE()),
('VR Headset', 6, 'Virtual reality headset for immersive experiences', 1700.00, 'Rented', 1, 2, GETDATE()),
('Tripod Stand', 4, 'Premium tripod stand', 1950.00, 'Available', 3, 5, GETDATE()),
('Monitor', 3, 'High-resolution display monitor', 600.00, 'Available', 4, 8, GETDATE());

-- Insert Admin User
INSERT INTO Users (Username, Email, FullName, PhoneNumber, PasswordHash, UserType, IsActive) VALUES
('admin', 'admin@rentalservice.com', 'Administrator', '09305797244', 'hashed_password_here', 'Admin', 1);

-- Insert Customer Users
INSERT INTO Users (Username, Email, FullName, PhoneNumber, PasswordHash, UserType, IsActive) VALUES
('christan.deguzman', 'christan@gmail.com', 'Christan De Guzman', '+1 (123) 456-7856', 'hashed_password_here', 'Customer', 1),
('joshua.paran', 'joshua@gmail.com', 'Joshua Paran', '09305797244', 'hashed_password_here', 'Customer', 1),
('gessel.carbajal', 'gessel@gmail.com', 'Gessel Mae Carbajal', '09305797244', 'hashed_password_here', 'Customer', 1),
('jennie.carvajal', 'jennie@gmail.com', 'Jennie Lorn Carvajal', '09305797244', 'hashed_password_here', 'Customer', 1),
('emiljay.tundag', 'emiljay@gmail.com', 'Emil Jay Tundag', '09919186125', 'hashed_password_here', 'Customer', 1),
('johary.cali', 'johary@gmail.com', 'Johary Cali', '09305797244', 'hashed_password_here', 'Customer', 1),
('kurt.limas', 'kurt@gmail.com', 'Kurt Russel Limas', '09305797244', 'hashed_password_here', 'Customer', 1),
('john.abilong', 'john@gmail.com', 'John Dave Abilong', '09305797244', 'hashed_password_here', 'Customer', 1),
('rheynan.marcelo', 'rheynan@gmail.com', 'Rheynan Rick Marcelo', '09305797244', 'hashed_password_here', 'Customer', 1),
('lyndon.hilona', 'lyndon@gmail.com', 'Lyndon Hilona', '09305797244', 'hashed_password_here', 'Customer', 1);

-- Insert Customers (Link users with customer details)
INSERT INTO Customers (UserId, Address, City, State, Status, TotalRentals) VALUES
(2, '123 Main St', 'Manila', 'NCR', 'Active', 5),
(3, '456 Oak Ave', 'Cebu', 'Cebu', 'Active', 2),
(4, '789 Pine Rd', 'Davao', 'Davao', 'VIP', 8),
(5, '321 Elm St', 'Quezon City', 'NCR', 'Active', 6),
(6, '654 Maple Dr', 'Makati', 'NCR', 'Active', 3),
(7, '987 Birch Ln', 'Pasig', 'NCR', 'Active', 4),
(8, '147 Cedar St', 'Manila', 'NCR', 'Active', 2),
(9, '258 Spruce Ave', 'Laguna', 'Calabarzon', 'Active', 5),
(10, '369 Walnut Rd', 'Rizal', 'Calabarzon', 'Active', 1),
(11, '741 Ash Ln', 'Cavite', 'Calabarzon', 'Active', 2);

-- Insert Payment Methods
INSERT INTO PaymentMethods (CustomerId, PaymentType, CardNumber, CardholderName, ExpiryDate, IsDefault, IsActive) VALUES
(1, 'Credit Card', '4242424242424242', 'Christan De Guzman', '12/26', 1, 1),
(1, 'GCash', NULL, 'Christan De Guzman', NULL, 0, 1),
(2, 'Credit Card', '5555555555555555', 'Joshua Paran', '08/26', 1, 1),
(3, 'Credit Card', '4111111111111111', 'Gessel Mae Carbajal', '06/27', 1, 1),
(4, 'Debit Card', '6011111111111111', 'Jennie Lorn Carvajal', '09/25', 1, 1),
(5, 'Digital Wallet', NULL, 'Emil Jay Tundag', NULL, 1, 1);

-- Insert Rentals
INSERT INTO Rentals (CustomerId, EquipmentId, RentalDate, ReturnDate, DueDate, RentalPrice, InsuranceFee, ProcessingFee, TotalPrice, Status, IsOverdue) VALUES
(1, 1, '2026-03-02', '2026-03-02', '2026-03-05', 850.00, 150.00, 100.00, 1100.00, 'Ongoing', 0),
(1, 3, '2026-03-04', '2026-03-02', '2026-03-06', 550.00, 150.00, 100.00, 800.00, 'Approved', 1),
(2, 2, '2026-03-01', '2026-03-02', '2026-03-03', 1100.00, 200.00, 100.00, 1400.00, 'Ongoing', 1),
(3, 8, '2026-03-09', '2026-03-02', '2026-03-12', 1700.00, 200.00, 150.00, 2050.00, 'Ongoing', 0),
(4, 4, '2026-03-03', '2026-03-02', '2026-03-05', 1400.00, 200.00, 100.00, 1700.00, 'Ongoing', 0),
(5, 6, '2026-03-05', '2026-03-02', '2026-03-08', 1000.00, 150.00, 100.00, 1250.00, 'Ongoing', 0),
(6, 5, '2026-03-07', '2026-03-02', '2026-03-09', 450.00, 100.00, 50.00, 600.00, 'Ongoing', 0),
(7, 9, '2026-03-11', '2026-03-02', '2026-03-13', 400.00, 100.00, 50.00, 550.00, 'Ongoing', 0),
(8, 7, '2026-03-08', '2026-03-02', '2026-03-10', 450.00, 100.00, 50.00, 600.00, 'Ongoing', 0),
(9, 10, '2026-03-06', '2026-03-02', '2026-03-09', 1700.00, 200.00, 150.00, 2050.00, 'Ongoing', 0);

-- Insert Payments
INSERT INTO Payments (RentalId, CustomerId, PaymentMethodId, Amount, Status, TransactionNumber) VALUES
(1, 1, 1, 1100.00, 'Completed', 'TXN001'),
(3, 2, 3, 1400.00, 'Completed', 'TXN002'),
(4, 3, 4, 2050.00, 'Pending', 'TXN003'),
(5, 4, 5, 1700.00, 'Completed', 'TXN004');

-- Insert Notifications
INSERT INTO Notifications (UserId, Title, Message, NotificationType, IsRead) VALUES
(1, 'Overdue Rental Alert', 'Equipment needs to be returned soon', 'Alert', 0),
(2, 'Payment Reminder', 'Your payment is due for rental ID 3', 'Reminder', 0),
(3, 'Rental Approved', 'Your rental request has been approved', 'Success', 1);

-- Insert Maintenance Logs
INSERT INTO MaintenanceLogs (EquipmentId, MaintenanceDate, Description, Status) VALUES
(4, '2026-03-04', 'Projector needs maintenance - check bulb', 'Scheduled', 'Scheduled'),
(2, '2026-03-05', 'Laptop screen cleaning and software update', 'Scheduled', 'In Progress');
