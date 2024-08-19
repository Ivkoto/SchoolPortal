IF EXISTS (SELECT 1 FROM sys.columns 
           WHERE object_id = OBJECT_ID(N'[Application].[SchoolYear]') 
           AND name = 'StartDate')
BEGIN
    ALTER TABLE [Application].[SchoolYear]
    DROP COLUMN [StartDate];
END
GO

IF EXISTS (SELECT 1 FROM sys.columns 
           WHERE object_id = OBJECT_ID(N'[Application].[SchoolYear]') 
           AND name = 'EndDate')
BEGIN
    ALTER TABLE [Application].[SchoolYear]
    DROP COLUMN [EndDate];
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns 
               WHERE object_id = OBJECT_ID(N'[Application].[SchoolYear]') 
               AND name = 'Year')
BEGIN
    ALTER TABLE [Application].[SchoolYear]
    ADD [Year] INT;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints 
               WHERE name = 'CK_Year' 
               AND parent_object_id = OBJECT_ID(N'[Application].[SchoolYear]'))
BEGIN
    ALTER TABLE [Application].[SchoolYear]
    ADD CONSTRAINT CK_Year CHECK ([Year] BETWEEN 2000 AND 3000);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns 
               WHERE object_id = OBJECT_ID(N'[Application].[Address]') 
               AND name = 'SettlementType')
BEGIN
    ALTER TABLE [Application].[Address]
    ADD [SettlementType] NVARCHAR(10);
END
GO

IF EXISTS (SELECT 1 FROM sys.columns 
           WHERE object_id = OBJECT_ID(N'[Application].[Address]') 
           AND name = 'PostalCode' 
           AND system_type_id = 231 -- NVARCHAR
           AND max_length <> 20) -- 10 * 2 since max_length is stored in bytes, NVARCHAR uses 2 bytes per character
BEGIN
    ALTER TABLE [Application].[Address]
    ALTER COLUMN [PostalCode] NVARCHAR(20);
END
GO
