-- Adding IsProtected column if it doesn't exist
IF NOT EXISTS (
    SELECT 1 
    FROM sys.columns 
    WHERE Name = N'IsProtected' 
    AND Object_ID = Object_ID(N'[Application].[Specialty]')
)
BEGIN
    ALTER TABLE [Application].[Specialty]
    ADD [IsProtected] BIT DEFAULT 0 NOT NULL;
END;

-- Adding HasExpectedShortage column if it doesn't exist
IF NOT EXISTS (
    SELECT 1 
    FROM sys.columns 
    WHERE Name = N'HasExpectedShortage' 
    AND Object_ID = Object_ID(N'[Application].[Specialty]')
)
BEGIN
    ALTER TABLE [Application].[Specialty]
    ADD [HasExpectedShortage] BIT DEFAULT 0 NOT NULL;
END;
