CREATE TABLE [Application].[EducationType_Ownership]
(
    [Id]              INT NOT NULL IDENTITY (1, 1),
    [EducationTypeId] INT NOT NULL,
    [OwnershipId]     INT NOT NULL,

    CONSTRAINT [PK_EducationTypeOwnership_Id] PRIMARY KEY CLUSTERED ([Id]),
    CONSTRAINT [FK_EducationTypeOwnership_EducationTypeId] FOREIGN KEY ([EducationTypeId]) REFERENCES [Application].[EducationType] ([Id]),
    CONSTRAINT [FK_EducationTypeOwnership_OwnershipId] FOREIGN KEY ([OwnershipId]) REFERENCES [Application].[InstitutionOwnership] ([Id]),
    CONSTRAINT [UQ_EducationTypeOwnership_EducationTypeId_OwnershipId] UNIQUE ([EducationTypeId], [OwnershipId])
);
GO

ALTER TABLE [Application].[EducationType]
DROP CONSTRAINT [FK_EducationType_OwnershipId];
GO

ALTER TABLE [Application].[EducationType]
DROP COLUMN [OwnershipId];
GO
