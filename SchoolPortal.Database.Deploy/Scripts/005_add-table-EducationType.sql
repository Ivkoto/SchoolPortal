
CREATE TABLE [Application].[EducationType]
(
	[Id] INT NOT NULL IDENTITY (1, 1),
	[OwnershipId] INT,
	[Name] NVARCHAR(100),

	CONSTRAINT [PK_EducationType_Id] PRIMARY KEY CLUSTERED ([Id]),
	CONSTRAINT [FK_EducationType_OwnershipId] FOREIGN KEY ([OwnershipId]) REFERENCES [Application].[InstitutionOwnership] (Id)
);
GO

DROP INDEX [UIDX_Address_Settlement_Neighbourhood] ON [Application].[Address];
DROP INDEX [UIDX_Address_Latitude_Longitude] ON [Application].[Address];
GO

ALTER TABLE [Application].[Address]
ALTER COLUMN [Latitude] DECIMAL(19, 17);
GO

ALTER TABLE [Application].[Address]
ALTER COLUMN [Longitude] DECIMAL(20, 17);
GO

CREATE INDEX [UIDX_Address_Settlement_Neighbourhood] ON [Application].[Address] (Settlement, Neighbourhood);
CREATE INDEX [UIDX_Address_Latitude_Longitude] ON [Application].[Address] (Latitude, Longitude);
GO
