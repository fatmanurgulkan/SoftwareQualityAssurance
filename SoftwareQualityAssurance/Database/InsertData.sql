-- Real Estate Management Database - Insert Data Script
-- Database: RealEstateManagementDb
-- Server: MELIH\SQLEXPRESS

USE RealEstateManagementDb;
GO

-- Insert Categories
INSERT INTO Categories (Name, Description, CreatedDate, IsDeleted) VALUES
('Apartment', 'Residential apartment units', GETUTCDATE(), 0),
('Villa', 'Luxury standalone houses', GETUTCDATE(), 0),
('Office', 'Commercial office spaces', GETUTCDATE(), 0),
('Shop', 'Retail shop spaces', GETUTCDATE(), 0),
('Land', 'Empty land plots', GETUTCDATE(), 0);
GO

-- Insert Locations
INSERT INTO Locations (CityName, PlateCode, CreatedDate, IsDeleted) VALUES
('Istanbul', '34', GETUTCDATE(), 0),
('Ankara', '06', GETUTCDATE(), 0),
('Izmir', '35', GETUTCDATE(), 0),
('Antalya', '07', GETUTCDATE(), 0),
('Bursa', '16', GETUTCDATE(), 0),
('Adana', '01', GETUTCDATE(), 0);
GO

-- Insert Customers
INSERT INTO Customers (FirstName, LastName, Email, IdentityNumber, Balance, PhoneNumber, CreatedDate, IsDeleted) VALUES
('Ahmet', 'Yılmaz', 'ahmet.yilmaz@email.com', '12345678901', 50000.00, '05551234567', GETUTCDATE(), 0),
('Ayşe', 'Demir', 'ayse.demir@email.com', '23456789012', 75000.00, '05552345678', GETUTCDATE(), 0),
('Mehmet', 'Kaya', 'mehmet.kaya@email.com', '34567890123', 100000.00, '05553456789', GETUTCDATE(), 0),
('Fatma', 'Şahin', 'fatma.sahin@email.com', '45678901234', 30000.00, '05554567890', GETUTCDATE(), 0),
('Ali', 'Çelik', 'ali.celik@email.com', '56789012345', 125000.00, '05555678901', GETUTCDATE(), 0),
('Zeynep', 'Arslan', 'zeynep.arslan@email.com', '67890123456', 60000.00, '05556789012', GETUTCDATE(), 0),
('Mustafa', 'Özdemir', 'mustafa.ozdemir@email.com', '78901234567', 90000.00, '05557890123', GETUTCDATE(), 0),
('Elif', 'Yıldız', 'elif.yildiz@email.com', '89012345678', 40000.00, '05558901234', GETUTCDATE(), 0);
GO

-- Insert Properties
INSERT INTO Properties (Title, BlockNumber, ParcelNumber, SquareMeters, Price, CategoryId, LocationId, IsAvailable, CreatedDate, IsDeleted) VALUES
('Modern 3+1 Apartment in Kadıköy', 'A-15', 'P-1234', 120.50, 2500000.00, 1, 1, 1, GETUTCDATE(), 0),
('Luxury Villa in Bebek', 'B-8', 'P-5678', 350.00, 8500000.00, 2, 1, 1, GETUTCDATE(), 0),
('Office Space in Levent', 'C-22', 'P-9012', 200.00, 3500000.00, 3, 1, 1, GETUTCDATE(), 0),
('Shop in Çankaya', 'D-5', 'P-3456', 80.00, 1200000.00, 4, 2, 1, GETUTCDATE(), 0),
('Land Plot in Çeşme', 'E-12', 'P-7890', 500.00, 2000000.00, 5, 3, 1, GETUTCDATE(), 0),
('2+1 Apartment in Çankaya', 'F-18', 'P-2468', 95.00, 1800000.00, 1, 2, 1, GETUTCDATE(), 0),
('Villa in Kemer', 'G-3', 'P-1357', 280.00, 4500000.00, 2, 4, 0, GETUTCDATE(), 0),
('Office in Alsancak', 'H-9', 'P-8024', 150.00, 2200000.00, 3, 3, 1, GETUTCDATE(), 0),
('Shop in Nilüfer', 'I-14', 'P-6801', 65.00, 950000.00, 4, 5, 1, GETUTCDATE(), 0),
('Land in Seyhan', 'J-7', 'P-3579', 300.00, 1500000.00, 5, 6, 1, GETUTCDATE(), 0);
GO

-- Insert Invoices
INSERT INTO Invoices (SerialNumber, TotalAmount, InvoiceDate, CustomerId, Status, CreatedDate, IsDeleted) VALUES
('INV-2024-001', 5000.00, '2024-01-15', 1, 'Paid', GETUTCDATE(), 0),
('INV-2024-002', 7500.00, '2024-01-20', 2, 'Paid', GETUTCDATE(), 0),
('INV-2024-003', 10000.00, '2024-02-01', 3, 'Pending', GETUTCDATE(), 0),
('INV-2024-004', 3000.00, '2024-02-10', 4, 'Paid', GETUTCDATE(), 0),
('INV-2024-005', 12500.00, '2024-02-15', 5, 'Paid', GETUTCDATE(), 0),
('INV-2024-006', 6000.00, '2024-03-01', 6, 'Pending', GETUTCDATE(), 0),
('INV-2024-007', 9000.00, '2024-03-05', 7, 'Paid', GETUTCDATE(), 0),
('INV-2024-008', 4000.00, '2024-03-10', 8, 'Cancelled', GETUTCDATE(), 0),
('INV-2024-009', 5500.00, '2024-03-15', 1, 'Pending', GETUTCDATE(), 0),
('INV-2024-010', 8000.00, '2024-03-20', 2, 'Paid', GETUTCDATE(), 0);
GO

PRINT 'Data insertion completed successfully!';
GO

