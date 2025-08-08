CREATE TABLE [Application].[ProfileDetails_EducationType]
(
    [Id]                INT NOT NULL IDENTITY (1, 1),
    [ProfileDetailsId]  INT NOT NULL,
    [EducationTypeId]   INT NOT NULL,

    CONSTRAINT [PK_ProfileDetailsEducationType_Id] PRIMARY KEY CLUSTERED ([Id]),
    CONSTRAINT [FK_ProfileDetailsEducationType_ProfileDetailsId] FOREIGN KEY ([ProfileDetailsId]) REFERENCES [Application].[ProfileDetails] ([Id]),
    CONSTRAINT [FK_ProfileDetailsEducationType_EducationTypeId] FOREIGN KEY ([EducationTypeId]) REFERENCES [Application].[EducationType] ([Id]),
    CONSTRAINT [UQ_ProfileDetailsEducationType_ProfileDetailsId_EducationTypeId] UNIQUE ([ProfileDetailsId], [EducationTypeId])
);
GO

ALTER TABLE [Application].[ProfileDetails]
DROP COLUMN [EducatingType];
GO