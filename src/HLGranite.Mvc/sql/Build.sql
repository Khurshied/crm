USE [Master]
IF NOT EXISTS(SELECT name FROM master.dbo.sysdatabases WHERE name = 'hlgranite')
CREATE database [hlgranite]
GO

--DROP TABLE Activities;
--DROP TABLE Statuses;
--DROP TABLE Slabs;
--DROP TABLE Tombs;
--DROP TABLE Nisans;
--DROP TABLE Stocks;
--DROP TABLE StockTypes;
--DROP TABLE Users;
--DROP TABLE UserTypes;


USE [hlgranite]

-- Create user role table
-- Admin, Staff, Agent, Customer
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[UserTypes]') AND OBJECTPROPERTY(id, N'IsTable') = 1)
CREATE TABLE [UserTypes] (
        [Id]                                    smallint IDENTITY NOT NULL,
        [Type]                                  nvarchar(50) NOT NULL
CONSTRAINT [PK_UserTypes] PRIMARY KEY ([Id])
)
GO

-- Create Users table
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Users]') AND OBJECTPROPERTY(id, N'IsTable') = 1)
CREATE TABLE [Users] (
        [Id]                                    int IDENTITY NOT NULL,
        [UserTypeId]                            smallint NOT NULL,
		[UserName]                              nvarchar(100) NOT NULL,
        [Password]                              nvarchar(100) NOT NULL,	--stored hash instead of pure text

        [FirstName]								nvarchar(100) NOT NULL,
		[LastName]								nvarchar(100) NOT NULL,
		[Email]                                 nvarchar(100),
		[Telephone]								nvarchar(100),
		[Mobile]								nvarchar(100),

		[Street1]                               nvarchar(100),
        [Street2]                               nvarchar(100),
        [City]                                  nvarchar(100),
        [Postcode]                              nvarchar(100),
		[State]									nvarchar(100),
		[Country]								nvarchar(100),

		[Remarks]								ntext,
		[Active]								bit NOT NULL DEFAULT 1
CONSTRAINT [PK_Users] PRIMARY KEY ([Id]),
CONSTRAINT [FK_Users_UserTypeId] FOREIGN KEY ([UserTypeId]) REFERENCES [UserTypes]([Id]),
)
GO

-- Create StockTypes table
-- Nisan, Fengshui, Renovation
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[StockTypes]') AND OBJECTPROPERTY(id, N'IsTable') = 1)
CREATE TABLE [StockTypes] (
        [Id]                                    smallint IDENTITY NOT NULL,
        [Type]									nvarchar(50) NOT NULL
CONSTRAINT [PK_StockTypes] PRIMARY KEY ([Id])
)
GO

-- Create Stock table
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Stocks]') AND OBJECTPROPERTY(id, N'IsTable') = 1)
CREATE TABLE [Stocks] (
        [Id]                                    int IDENTITY NOT NULL,
        [StockTypeId]							smallint NOT NULL,
        [Code]                                  nvarchar(50) NOT NULL, --This become a key link to Stocks fusion table for Warehouse Scanner app
        [Description]							nvarchar(50),
        [Url]                                   nvarchar(255),  --stored image url
        [Price]                                 money NOT NULL DEFAULT 0,
		[Remarks]								ntext,
		[Active]								bit NOT NULL DEFAULT 1
CONSTRAINT [PK_Stocks] PRIMARY KEY ([Id]),
CONSTRAINT [FK_Stocks_StockTypeId] FOREIGN KEY ([StockTypeId]) REFERENCES [StockTypes]([Id])
)
GO

-- Create Nisan table
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Nisans]') AND OBJECTPROPERTY(id, N'IsTable') = 1)
CREATE TABLE [Nisans] (
        [Id]                                    int IDENTITY NOT NULL,
		[StockId]								int NOT NULL,
        [SoldToId]								int NOT NULL,
		[Rumi]									nvarchar(100),
		[Jawi]									nvarchar(100),
		[Born]									datetime,
		[Death]									datetime,
		[Deathm]								datetime,
		[Remarks]								ntext
CONSTRAINT [PK_Nisans] PRIMARY KEY ([Id]),
CONSTRAINT [FK_Nisans_StockId] FOREIGN KEY ([StockId]) REFERENCES [Stocks]([Id]),
CONSTRAINT [FK_Nisans_SoldToId] FOREIGN KEY ([SoldToId]) REFERENCES [Users]([Id])
)
GO

-- Create Granite slab table
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Slabs]') AND OBJECTPROPERTY(id, N'IsTable') = 1)
CREATE TABLE [Slabs] (
        [Id]                                    int IDENTITY NOT NULL,
		[StockId]								int NOT NULL,
        [SoldToId]								int NOT NULL,
		[Reference]								nvarchar(100), -- refer to project name
		[Length]								int,
		[Width]									int,
		[Height]								int,
		[Remarks]								ntext
CONSTRAINT [PK_Slabs] PRIMARY KEY ([Id]),
CONSTRAINT [FK_Slabs_StockId] FOREIGN KEY ([StockId]) REFERENCES [Stocks]([Id]),
CONSTRAINT [FK_Slabs_SoldToId] FOREIGN KEY ([SoldToId]) REFERENCES [Users]([Id])
)
GO

-- Create fengshui table
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Fengshuis]') AND OBJECTPROPERTY(id, N'IsTable') = 1)
CREATE TABLE [Tombs] (
        [Id]                                    int IDENTITY NOT NULL,
		[StockId]								int NOT NULL,
        [SoldToId]								int NOT NULL,
		[Reference]								nvarchar(100), -- refer to project name
		[Remarks]								ntext
CONSTRAINT [PK_Tombs] PRIMARY KEY ([Id]),
CONSTRAINT [FK_Tomb_StockId] FOREIGN KEY ([StockId]) REFERENCES [Stocks]([Id]),
CONSTRAINT [FK_Tombs_SoldToId] FOREIGN KEY ([SoldToId]) REFERENCES [Users]([Id])
)
GO

-- Create Statuses table
-- New, Open, Cut, Delivered, Paid
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Statuses]') AND OBJECTPROPERTY(id, N'IsTable') = 1)
CREATE TABLE [Statuses] (
        [Id]                                    smallint IDENTITY NOT NULL,
        [Status]								nvarchar(50) NOT NULL,
		[StockTypeId]							smallint NOT NULL,
CONSTRAINT [PK_Statuses] PRIMARY KEY ([Id]),
CONSTRAINT [FK_Statuses_StockTypeId] FOREIGN KEY ([StockTypeId]) REFERENCES [StockTypes]([Id])
)
GO

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Activities]') AND OBJECTPROPERTY(id, N'IsTable') = 1)
CREATE TABLE [Activities] (
        [Id]                                    bigint IDENTITY NOT NULL,
		[OrderId]								int NOT NULL,
		[StockTypeId]							smallint NOT NULL,
        [StatusId]								smallint NOT NULL,
		[UserId]								int NOT NULL,
		[Date]									datetime NOT NULL
CONSTRAINT [PK_Activities] PRIMARY KEY ([Id]),
CONSTRAINT [FK_Activities_StockTypeId] FOREIGN KEY ([StockTypeId]) REFERENCES [StockTypes]([Id]),
CONSTRAINT [FK_Activities_StatusId] FOREIGN KEY ([StatusId]) REFERENCES [Statuses]([Id]),
CONSTRAINT [FK_Activities_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id])
)
GO



-- Initial default data
INSERT INTO [UserTypes](Type) VALUES ('Admin');
INSERT INTO [UserTypes](Type) VALUES ('Staff');
INSERT INTO [UserTypes](Type) VALUES ('Agent');
INSERT INTO [UserTypes](Type) VALUES ('Customer');

INSERT INTO [StockTypes](Type) VALUES ('Renovation');
INSERT INTO [StockTypes](Type) VALUES ('Tomb');
INSERT INTO [StockTypes](Type) VALUES ('Nisan');

INSERT INTO [Statuses](Status,StockTypeId) VALUES ('New', 3);
INSERT INTO [Statuses](Status,StockTypeId) VALUES ('Design', 3);
INSERT INTO [Statuses](Status,StockTypeId) VALUES ('Cut', 3);
INSERT INTO [Statuses](Status,StockTypeId) VALUES ('Complete', 3);
INSERT INTO [Statuses](Status,StockTypeId) VALUES ('Deliver', 3);
INSERT INTO [Statuses](Status,StockTypeId) VALUES ('Close', 3);
INSERT INTO [Statuses](Status,StockTypeId) VALUES ('Paid', 3);

INSERT INTO [Statuses](Status,StockTypeId) VALUES ('New', 1);
INSERT INTO [Statuses](Status,StockTypeId) VALUES ('Start', 1);
INSERT INTO [Statuses](Status,StockTypeId) VALUES ('Profiling', 1);
INSERT INTO [Statuses](Status,StockTypeId) VALUES ('Deliver', 1);
INSERT INTO [Statuses](Status,StockTypeId) VALUES ('Installing', 1);
INSERT INTO [Statuses](Status,StockTypeId) VALUES ('Close', 1);
INSERT INTO [Statuses](Status,StockTypeId) VALUES ('Paid', 1);