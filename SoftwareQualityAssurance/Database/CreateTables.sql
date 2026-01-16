-- Real Estate Management Database - Create Tables Script
-- Server: MELIH\SQLEXPRESS
-- Database: RealEstateManagementDb

USE RealEstateManagementDb;
GO

-- Drop tables if they exist (in reverse order of dependencies)
IF OBJECT_ID('Invoices', 'U') IS NOT NULL DROP TABLE Invoices;
IF OBJECT_ID('Properties', 'U') IS NOT NULL DROP TABLE Properties;
IF OBJECT_ID('Customers', 'U') IS NOT NULL DROP TABLE Customers;
IF OBJECT_ID('Categories', 'U') IS NOT NULL DROP TABLE Categories;
IF OBJECT_ID('Locations', 'U') IS NOT NULL DROP TABLE Locations;
GO

-- Create Categories Table
CREATE TABLE Categories (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    ModifiedDate DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);
GO

-- Create Locations Table
CREATE TABLE Locations (
    Id INT PRIMARY KEY IDENTITY(1,1),
    CityName NVARCHAR(100) NOT NULL,
    PlateCode NVARCHAR(10) NOT NULL,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    ModifiedDate DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);
GO

-- Create Customers Table
CREATE TABLE Customers (
    Id INT PRIMARY KEY IDENTITY(1,1),
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    IdentityNumber NVARCHAR(20),
    Balance DECIMAL(18,2) NOT NULL DEFAULT 0,
    PhoneNumber NVARCHAR(20),
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    ModifiedDate DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);
GO

-- Create Properties Table
CREATE TABLE Properties (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(200) NOT NULL,
    BlockNumber NVARCHAR(50),
    ParcelNumber NVARCHAR(50),
    SquareMeters DECIMAL(18,2) NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    CategoryId INT NOT NULL,
    LocationId INT NOT NULL,
    IsAvailable BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    ModifiedDate DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_Properties_Categories FOREIGN KEY (CategoryId) REFERENCES Categories(Id),
    CONSTRAINT FK_Properties_Locations FOREIGN KEY (LocationId) REFERENCES Locations(Id)
);
GO

-- Create Invoices Table
CREATE TABLE Invoices (
    Id INT PRIMARY KEY IDENTITY(1,1),
    SerialNumber NVARCHAR(50) NOT NULL,
    TotalAmount DECIMAL(18,2) NOT NULL,
    InvoiceDate DATETIME2 NOT NULL,
    CustomerId INT NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    ModifiedDate DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_Invoices_Customers FOREIGN KEY (CustomerId) REFERENCES Customers(Id)
);
GO

-- Create Indexes
CREATE INDEX IX_Customers_Email ON Customers(Email);
CREATE INDEX IX_Properties_CategoryId ON Properties(CategoryId);
CREATE INDEX IX_Properties_LocationId ON Properties(LocationId);
CREATE INDEX IX_Properties_IsAvailable ON Properties(IsAvailable);
CREATE INDEX IX_Invoices_CustomerId ON Invoices(CustomerId);
CREATE INDEX IX_Invoices_InvoiceDate ON Invoices(InvoiceDate);
GO

PRINT 'All tables created successfully!';
GO

