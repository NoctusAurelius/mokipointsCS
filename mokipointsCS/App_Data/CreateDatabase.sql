-- Mokipoints Database Creation Script
-- Run this script in SQL Server Management Studio or via Visual Studio Server Explorer

-- Create Database (if not exists)
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'Mokipoints')
BEGIN
    CREATE DATABASE Mokipoints
END
GO

USE Mokipoints
GO

-- Users Table - Stores family members/users
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Users] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [FirstName] NVARCHAR(50) NOT NULL,
        [LastName] NVARCHAR(50) NOT NULL,
        [MiddleName] NVARCHAR(50) NULL,
        [Email] NVARCHAR(255) NOT NULL UNIQUE,
        [Password] NVARCHAR(255) NOT NULL,
        [Birthday] DATETIME NULL,
        [Role] NVARCHAR(20) NOT NULL,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [IsActive] BIT NOT NULL DEFAULT 1
    )
    CREATE UNIQUE INDEX IX_Users_Email ON [dbo].[Users]([Email])
END
ELSE
BEGIN
    -- Add new columns if they don't exist (for existing databases)
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'FirstName')
    BEGIN
        ALTER TABLE [dbo].[Users] ADD [FirstName] NVARCHAR(50) NULL;
        ALTER TABLE [dbo].[Users] ADD [LastName] NVARCHAR(50) NULL;
        ALTER TABLE [dbo].[Users] ADD [MiddleName] NVARCHAR(50) NULL;
        ALTER TABLE [dbo].[Users] ADD [Birthday] DATETIME NULL;
        ALTER TABLE [dbo].[Users] ADD [Role] NVARCHAR(20) NULL;
        -- Migrate existing Name to FirstName if Name column exists
        IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'Name')
        BEGIN
            UPDATE [dbo].[Users] SET [FirstName] = [Name], [LastName] = '', [Role] = 'PARENT' WHERE [FirstName] IS NULL;
        END
        ELSE
        BEGIN
            UPDATE [dbo].[Users] SET [FirstName] = 'User', [LastName] = '', [Role] = 'PARENT' WHERE [FirstName] IS NULL;
        END
        ALTER TABLE [dbo].[Users] ALTER COLUMN [FirstName] NVARCHAR(50) NOT NULL;
        ALTER TABLE [dbo].[Users] ALTER COLUMN [LastName] NVARCHAR(50) NOT NULL;
        ALTER TABLE [dbo].[Users] ALTER COLUMN [Role] NVARCHAR(20) NOT NULL;
    END
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'Password')
    BEGIN
        ALTER TABLE [dbo].[Users] ADD [Password] NVARCHAR(255) NULL;
        UPDATE [dbo].[Users] SET [Password] = 'changeme' WHERE [Password] IS NULL;
        ALTER TABLE [dbo].[Users] ALTER COLUMN [Password] NVARCHAR(255) NOT NULL;
    END
    -- Make Email unique if not already
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Email' AND object_id = OBJECT_ID(N'[dbo].[Users]'))
    BEGIN
        CREATE UNIQUE INDEX IX_Users_Email ON [dbo].[Users]([Email]);
    END
END
GO

-- Chores Table - Stores available chores
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Chores]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Chores] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [Name] NVARCHAR(200) NOT NULL,
        [Description] NVARCHAR(MAX) NULL,
        [Points] INT NOT NULL DEFAULT 0,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [IsActive] BIT NOT NULL DEFAULT 1
    )
END
GO

-- ChoreAssignments Table - Tracks which chores are assigned to which users
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ChoreAssignments]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ChoreAssignments] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [UserId] INT NOT NULL,
        [ChoreId] INT NOT NULL,
        [AssignedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [CompletedDate] DATETIME NULL,
        [Status] NVARCHAR(50) NOT NULL DEFAULT 'Pending', -- Pending, Completed, Cancelled
        FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id]),
        FOREIGN KEY ([ChoreId]) REFERENCES [dbo].[Chores]([Id])
    )
END
GO

-- PointTransactions Table - Tracks all point transactions (earned/spent)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PointTransactions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[PointTransactions] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [UserId] INT NOT NULL,
        [Points] INT NOT NULL,
        [TransactionType] NVARCHAR(50) NOT NULL, -- Earned, Spent
        [Description] NVARCHAR(500) NULL,
        [TransactionDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [ChoreAssignmentId] INT NULL, -- Links to ChoreAssignment if points earned from completing a chore
        FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id]),
        FOREIGN KEY ([ChoreAssignmentId]) REFERENCES [dbo].[ChoreAssignments]([Id])
    )
END
GO

-- Create Indexes for better performance
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ChoreAssignments_UserId')
    CREATE INDEX IX_ChoreAssignments_UserId ON [dbo].[ChoreAssignments]([UserId])
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ChoreAssignments_ChoreId')
    CREATE INDEX IX_ChoreAssignments_ChoreId ON [dbo].[ChoreAssignments]([ChoreId])
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PointTransactions_UserId')
    CREATE INDEX IX_PointTransactions_UserId ON [dbo].[PointTransactions]([UserId])
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PointTransactions_TransactionDate')
    CREATE INDEX IX_PointTransactions_TransactionDate ON [dbo].[PointTransactions]([TransactionDate])
GO

-- Insert Sample Data (Optional - for testing)
-- Uncomment below to add sample data

/*
-- Sample Users
INSERT INTO [dbo].[Users] ([Name], [Email]) VALUES 
    ('John Doe', 'john@example.com'),
    ('Jane Doe', 'jane@example.com'),
    ('Kid One', 'kid1@example.com'),
    ('Kid Two', 'kid2@example.com')
GO

-- Sample Chores
INSERT INTO [dbo].[Chores] ([Name], [Description], [Points]) VALUES 
    ('Take out trash', 'Take all trash bags to the curb', 10),
    ('Wash dishes', 'Wash and dry all dishes', 15),
    ('Make bed', 'Make your bed in the morning', 5),
    ('Clean room', 'Pick up and organize your room', 20),
    ('Walk the dog', 'Take the dog for a 15-minute walk', 10),
    ('Do homework', 'Complete all assigned homework', 25)
GO
*/

PRINT 'Mokipoints database schema created successfully!'
GO

