-- Real Estate Management Database - Create Database Script
-- Server: MELIH\SQLEXPRESS

-- Create Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'RealEstateManagementDb')
BEGIN
    CREATE DATABASE RealEstateManagementDb;
    PRINT 'Database RealEstateManagementDb created successfully.';
END
ELSE
BEGIN
    PRINT 'Database RealEstateManagementDb already exists.';
END
GO

USE RealEstateManagementDb;
GO

-- Note: Tables will be created automatically by Entity Framework Core migrations
-- Run the following command in Package Manager Console:
-- Add-Migration InitialCreate
-- Update-Database

PRINT 'Database setup completed. Please run EF Core migrations to create tables.';
GO

