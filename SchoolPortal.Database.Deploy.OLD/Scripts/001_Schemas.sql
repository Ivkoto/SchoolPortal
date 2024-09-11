
IF NOT EXISTS ( SELECT * FROM sys.schemas WHERE name = N'[Applications]' )
    BEGIN
        EXEC ( 'CREATE SCHEMA [Applications] AUTHORIZATION [dbo]' )
    END
GO
