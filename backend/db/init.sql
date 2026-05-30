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
