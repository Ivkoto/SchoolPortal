IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[Application].[Address]') AND name = 'PostalCode')
BEGIN
    ALTER TABLE [Application].[Address]
    ALTER COLUMN [PostalCode] NVARCHAR(20);
END
GO;

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[Application].[Science]') AND name = 'ExternalId')
BEGIN
    ALTER TABLE [Application].[Science]
    ALTER COLUMN [ExternalId] INT;
END
GO;

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[Application].[ProfessionalDirection]') AND name = 'ExternalId')
BEGIN
    ALTER TABLE [Application].[ProfessionalDirection]
    ALTER COLUMN [ExternalId] INT;
END
GO;

IF EXISTS (SELECT 1FROM sys.columns WHERE object_id = OBJECT_ID(N'[Application].[Profession]') AND name = 'ExternalId')
BEGIN
    ALTER TABLE [Application].[Profession]
    ALTER COLUMN [ExternalId] INT;
END
GO;

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[Application].[Specialty]') AND name = 'ExternalId')
BEGIN
    ALTER TABLE [Application].[Specialty]
    ALTER COLUMN [ExternalId] INT;
END
GO;

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[Application].[Profile]') AND name = 'ExternalId')
BEGIN
    ALTER TABLE [Application].[Profile]
    ALTER COLUMN [ExternalId] INT;
END
GO;

IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Profile_SubInstitutionId' AND parent_object_id = OBJECT_ID(N'[Application].[Profile]'))
BEGIN
    ALTER TABLE [Application].[Profile]
    DROP CONSTRAINT [FK_Profile_SubInstitutionId];
END
GO;

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[Application].[Profile]') AND name = 'SubInstitutionId')
BEGIN
    ALTER TABLE [Application].[Profile]
    ALTER COLUMN [SubInstitutionId] INT;
END
ELSE
BEGIN
    ALTER TABLE [Application].[Profile]
    ADD [SubInstitutionId] INT;
END
GO;

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[Application].[ExamResult]') AND name = 'Grade')
BEGIN
    ALTER TABLE [Application].[ExamResult]
    ALTER COLUMN [Grade] INT;
END
ELSE
BEGIN
    ALTER TABLE [Application].[ExamResult]
    ADD [Grade] INT;
END
GO;

ALTER TABLE [Application].[Profile]
ADD CONSTRAINT [FK_Profile_SubInstitutionId] FOREIGN KEY ([SubInstitutionId]) REFERENCES [Application].[SubInstitution] ([Id]);
GO;

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[Application].[ExamStage]') AND name = 'StageNumber')
BEGIN
    ALTER TABLE [Application].[ExamStage]
    ALTER COLUMN [StageNumber] INT;
END
ELSE
BEGIN
    ALTER TABLE [Application].[ExamStage]
    ADD [StageNumber] INT;
END
GO;


--free profile positions by application stage
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[Application].[ExamStage]') AND name = 'FreePositionsTotal')
BEGIN
    ALTER TABLE [Application].[ExamStage]
    ALTER COLUMN [FreePositionsTotal] INT;
END
ELSE
BEGIN
    ALTER TABLE [Application].[ExamStage]
    ADD [FreePositionsTotal] INT;
END
GO;

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[Application].[ExamStage]') AND name = 'FreePositionsMen')
BEGIN
    ALTER TABLE [Application].[ExamStage]
    ALTER COLUMN [FreePositionsMen] INT;
END
ELSE
BEGIN
    ALTER TABLE [Application].[ExamStage]
    ADD [FreePositionsMen] INT;
END
GO;

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[Application].[ExamStage]') AND name = 'FreePositionsWomen')
BEGIN
    ALTER TABLE [Application].[ExamStage]
    ALTER COLUMN [FreePositionsWomen] INT;
END
ELSE
BEGIN
    ALTER TABLE [Application].[ExamStage]
    ADD [FreePositionsWomen] INT;
END
GO;