IF NOT EXISTS (
    SELECT 1 
    FROM sys.columns 
    WHERE Name = N'Neighbourhood' 
    AND Object_ID = Object_ID(N'[Application].[Address]')
)
BEGIN
    ALTER TABLE [Application].[Address]
    ADD [Neighbourhood] NVARCHAR(100);
END