IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'BallastLaneDb')
BEGIN
    CREATE DATABASE BallastLaneDb;
END
GO

USE BallastLaneDb;
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        Id          INT IDENTITY(1,1) PRIMARY KEY,
        Username    NVARCHAR(100)  NOT NULL,
        PasswordHash NVARCHAR(500) NOT NULL,
        Email       NVARCHAR(200)  NOT NULL,
        CreatedAt   DATETIME2      NOT NULL
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Products')
BEGIN
    CREATE TABLE Products (
        Id          INT IDENTITY(1,1) PRIMARY KEY,
        Name        NVARCHAR(200)  NOT NULL,
        Description NVARCHAR(1000) NOT NULL,
        Price       DECIMAL(18, 2) NOT NULL,
        Stock       INT            NOT NULL,
        CreatedAt   DATETIME2      NOT NULL,
        UpdatedAt   DATETIME2      NULL
    );
END
GO

-- Seed 5 realistic default products if the table is empty
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Products')
BEGIN
    IF NOT EXISTS (SELECT * FROM Products)
    BEGIN
        INSERT INTO Products (Name, Description, Price, Stock, CreatedAt)
        VALUES 
        ('Wireless Mechanical Keyboard', 'Compact 75% layout with hot-swappable tactile switches and dynamic RGB backlighting.', 89.99, 120, GETUTCDATE()),
        ('Ergonomic Wireless Mouse', 'High-precision tracking sensor with silent clicks and ergonomic hand support.', 49.99, 200, GETUTCDATE()),
        ('UltraWide Curved Monitor', '34-inch 1440p resolution IPS display with 144Hz refresh rate and HDR support.', 349.99, 45, GETUTCDATE()),
        ('Noise-Cancelling Headphones', 'Over-ear Bluetooth headphones with active hybrid noise cancellation and 40-hour battery life.', 199.99, 85, GETUTCDATE()),
        ('USB-C Dual HDMI Docking Station', 'Multi-port adapter supporting dual 4K monitors, gigabit Ethernet, and 100W Power Delivery.', 79.99, 150, GETUTCDATE());
    END
END
GO
