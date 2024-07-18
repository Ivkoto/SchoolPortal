
IF NOT EXISTS ( SELECT * FROM sys.schemas WHERE name = N'[Application]' )
    BEGIN
        EXEC ( 'CREATE SCHEMA [Application] AUTHORIZATION [dbo]' )
    END
GO
