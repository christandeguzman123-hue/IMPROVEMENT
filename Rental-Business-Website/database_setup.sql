-- Rental Business System Database Script
-- Run this script in XAMPP phpMyAdmin to create the database and tables

-- Create Database
CREATE DATABASE IF NOT EXISTS rental_business_db;
USE rental_business_db;

-- Create Admins Table
CREATE TABLE IF NOT EXISTS admins (
    id INT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    email_address VARCHAR(100),
    created_date DATETIME DEFAULT CURRENT_TIMESTAMP,
    is_active BOOLEAN DEFAULT TRUE
);

-- Create Rental Items Table
CREATE TABLE IF NOT EXISTS rental_items (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    description TEXT,
    daily_rate DECIMAL(10, 2) NOT NULL,
    quantity_available INT NOT NULL,
    category VARCHAR(50) NOT NULL,
    image_path VARCHAR(255),
    created_date DATETIME DEFAULT CURRENT_TIMESTAMP,
    is_active BOOLEAN DEFAULT TRUE,
    INDEX idx_category (category),
    INDEX idx_active (is_active)
);

-- Create Rentals Table
CREATE TABLE IF NOT EXISTS rentals (
    id INT AUTO_INCREMENT PRIMARY KEY,
    item_id INT NOT NULL,
    customer_name VARCHAR(100) NOT NULL,
    customer_email VARCHAR(100) NOT NULL,
    customer_phone VARCHAR(20) NOT NULL,
    rental_start_date DATE NOT NULL,
    rental_end_date DATE NOT NULL,
    quantity INT NOT NULL,
    total_cost DECIMAL(10, 2) NOT NULL,
    payment_method VARCHAR(50) NOT NULL,
    payment_status VARCHAR(20) DEFAULT 'Pending',
    rental_status VARCHAR(20) DEFAULT 'Active',
    created_date DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (item_id) REFERENCES rental_items(id),
    INDEX idx_status (rental_status),
    INDEX idx_created (created_date)
);

-- Create Payments Table
CREATE TABLE IF NOT EXISTS payments (
    id INT AUTO_INCREMENT PRIMARY KEY,
    rental_id INT NOT NULL,
    amount DECIMAL(10, 2) NOT NULL,
    payment_method VARCHAR(50) NOT NULL,
    transaction_id VARCHAR(100),
    payment_date DATETIME DEFAULT CURRENT_TIMESTAMP,
    payment_status VARCHAR(20) DEFAULT 'Success',
    notes TEXT,
    FOREIGN KEY (rental_id) REFERENCES rentals(id),
    INDEX idx_status (payment_status)
);

-- Insert Default Admin User
-- Username: admin
-- Password: admin123
INSERT INTO admins (username, password_hash, email_address) 
VALUES ('admin', MD5('admin123'), 'admin@rentalbizsystem.com');

-- Insert Sample Rental Items
INSERT INTO rental_items (name, description, daily_rate, quantity_available, category) VALUES
('Canon EOS 5D Mark IV', 'Professional DSLR Camera - 4K capable', 50.00, 5, 'Camera'),
('Sony Projector VPL-FHZ100', 'Full HD Projector - 3000 Lumens', 30.00, 3, 'Projector'),
('Bose SoundLink Revolve', 'Portable Bluetooth Speaker', 15.00, 8, 'Speaker'),
('MacBook Pro 16', 'Laptop - 16GB RAM, M2 Pro', 45.00, 4, 'Laptop'),
('Shure SM7B Microphone', 'Professional Studio Microphone', 25.00, 6, 'Microphone'),
('RGB Lighting Kit', 'Complete LED Lighting Setup - 4 Panels', 20.00, 7, 'Lighting Kit'),
('Manfrotto Tripod', 'Heavy Duty Photography Tripod', 12.00, 10, 'Tripod'),
('DJI Phantom 4 Pro', 'Professional Drone with 4K Camera', 75.00, 2, 'Drone'),
('iPad Pro 12.9', 'Tablet - 256GB, WiFi + Cellular', 35.00, 5, 'Tablet'),
('Meta Quest 3', 'VR Headset - 128GB', 40.00, 3, 'VR Headset');

-- Create Index for better performance
CREATE INDEX idx_rental_dates ON rentals(rental_start_date, rental_end_date);
